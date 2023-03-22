using Microsoft.AspNetCore.Mvc;
using SellBooksNow.Gateway.Models;
using SellBooksNow.Gateway.Dtos;
using VirtualLibrary.Books.Services.Event;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SellBooksNow.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private static readonly List<Transaction> _transactions = new List<Transaction>();
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBookService _bookService;

        public GatewayController(IBookService bookService)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);
            _bookService = bookService;
        }

        [HttpGet("/books")]
        public async Task<IActionResult> GetBooks()
        {
            var result = await _bookService.GetBooksAsync();
            return Ok(result);
        }

        [HttpPost("/order")]
        public async Task<IActionResult> AddOrder([FromBody] BookInformationDataTransferObjet bookInformation, [FromBody] PaymentMethodInformationDataTransferObject paymentMethodInformation)
        {
            var books = await _bookService.GetBooksAsync();
            var book = books.SingleOrDefault(x => x.Isbn == bookInformation.Isbn);
            
            if (book != null)
            {
                return BadRequest($"El Libro con ISBN: {bookInformation.Isbn} no existe");
            }else if (book.NumberOfCopies <= 0)
            {
                return BadRequest($"El Libro con ISBN: {bookInformation.Isbn} no tiene existencias disponibles en este momento");
            }
            else
            {                
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    Status = TransactionStatus.InProcess,
                };

                _transactions.Add(transaction);

                var billing_information = new BillingDataTransferObject
                {
                    book_information = bookInformation,
                    payment_method = paymentMethodInformation,
                    transaction = transaction,
                };

                sendBillingInformation(billing_information);
                sendBookInformation(bookInformation);

                return Ok(transaction);
            }
        }

        [HttpGet("/transactions/{id_transaction}")]
        public IActionResult GetTransaction(Guid id_transaction)
        {
            var result = _transactions.SingleOrDefault(x => x.Id == id_transaction);
            return Ok(result);
        }

        private void sendBillingInformation(BillingDataTransferObject billing_information)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("billing-information", false, false, false, null);
                    var json = JsonConvert.SerializeObject(billing_information);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(string.Empty, "billing-information", null, body);
                }
            }
        }

        private void sendBookInformation(BookInformationDataTransferObjet book_information)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("books-queue", false, false, false, null);
                    var json = JsonConvert.SerializeObject(book_information);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(string.Empty, "books-queue", null, body);
                }
            }
        }

        public void UpdateTransaction(CancellationToken cancellationToken)
        {
            _channel.QueueDeclare("transaction_result", false, false, false, null);
            _consumer.Received += async (model, content) =>
            {
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var transaction = _transactions.SingleOrDefault(x => x.Id.ToString().Equals(json));
                var new_transaction = new Transaction
                {
                    Id = transaction.Id,
                    Status = TransactionStatus.Completed,
                };

                int indice = _transactions.FindIndex(x => x.Id.ToString().Equals(json));
                _transactions.RemoveAt(indice);
                _transactions.Insert(indice, new_transaction);

            };
            _channel.BasicConsume("transaction_result", true, _consumer);
        }
    }
}