using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();

app.Run();

