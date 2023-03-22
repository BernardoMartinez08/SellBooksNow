using System.Text;
using System.Threading.Channels;
using System.Transactions;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SellBooksNow.Shipping.Dtos;


namespace SellBooksNow.Shipping;

public class PaymentService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IModel _channelB;
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
        _channelB = _connection.CreateModel();
        _channel.QueueDeclare("books-queue", false, false, false, null);
        _channelB.QueueDeclare("payment-results", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Received += async (model, content) =>
        {
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var billingInformation = JsonConvert.DeserializeObject<BookDataTransferObject>(json);
        };
        _channel.BasicConsume("books-queu", true, _consumer);

        _consumer.Received += async (model, content) =>
        {
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            if (json != "")
                NotifyPaymentResult(json);
        };
        _channel.BasicConsume("payment-results", true, _consumer);

        return Task.CompletedTask;
    }

    private void NotifyPaymentResult(string id_transaction)
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
                var json = JsonConvert.SerializeObject(id_transaction);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(string.Empty, "transaction_result", null, body);
            }
        }
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