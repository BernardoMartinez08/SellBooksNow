using Microsoft.AspNetCore.Mvc;
using VirtualLibrary.Books.Services.Event;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient<IBookService, BookService>();
var app = builder.Build();
app.MapControllers();

app.Run();