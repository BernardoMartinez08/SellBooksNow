using System.Text;
using System.Transactions;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SellBooksNow.Billing.Dtos;
using SellBooksNow.Billing.Models;
using Transaction = SellBooksNow.Billing.Models.Transaction;
using TransactionStatus = SellBooksNow.Billing.Models.TransactionStatus;

namespace SellBooksNow.Billing;
public class PaymentService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;

    public PaymentService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare("billing-information", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Received += async (model, content) =>
        {
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var billingInformation = JsonConvert.DeserializeObject<BillingDataTransferObject>(json);
            var paymentResult = await ProcessPayment(billingInformation, cancellationToken);
            var message = $"El pago para el libro {billingInformation.book_information.Isbn} se proceso con estado {paymentResult.transaction.Status}";
            Console.WriteLine(message);
        };
        _channel.BasicConsume("billing-information", true, _consumer);
        return Task.CompletedTask;
    }

    private void NotifyPaymentResult(Guid id_transaction)
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
                channel.QueueDeclare("payment-results", false, false, false, null);
                var json = JsonConvert.SerializeObject(id_transaction);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(string.Empty, "payment-results", null, body);
            }
        }
    }

    private void NotifyPaymentProcess(Guid id_transaction)
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
                channel.QueueDeclare("transaction_result", false, false, false, null);
                var json = JsonConvert.SerializeObject(id_transaction.ToString());
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(string.Empty, "transaction_result", null, body);
            }
        }
    }

    private async Task<BillingDataTransferObject> ProcessPayment(BillingDataTransferObject billingInformation,CancellationToken token)
    {
        var errors = new List<string>();

        if (billingInformation.payment_method.CardNumber == Guid.Empty)
        {
            errors.Add("La tarjeta debe tener un numero valido.");
        }

        await Task.Delay(2000, token);

        if (billingInformation.payment_method.Cvv <= 0)
        {
            errors.Add("La tarjeta debe tener un cvv valido.");
        }

        await Task.Delay(2000, token);

        if (billingInformation.payment_method.Month <= DateTime.Today.Month || billingInformation.payment_method.Year < 0)
        {
            errors.Add("La tarjeta debe tener una fecha de expiracion valida.");
        }

        if(errors.Any()){
            billingInformation.transaction.Status = TransactionStatus.Charged;
            NotifyPaymentProcess(billingInformation.transaction.Id);
            NotifyPaymentResult(billingInformation.transaction.Id);
        }

        return billingInformation;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Servicio de pagos ejecutandose {DateTimeOffset.Now}");
            await Task.Delay(1000, stoppingToken);
        }
    }
}