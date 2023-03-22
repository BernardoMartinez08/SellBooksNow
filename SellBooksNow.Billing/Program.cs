using Microsoft.AspNetCore.Mvc;
using TicketShop.Payments;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHostedService<PaymentService>();

var app = builder.Build();
app.MapControllers();

app.Run();