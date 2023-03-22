    using Microsoft.Extensions.Options;
using SellBooksNow.Gateway.Dtos;
using Newtonsoft.Json;

namespace VirtualLibrary.Books.Services.Event;

public class BookService : IBookService
{
    public readonly HttpClient _httpClient;

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<BookDataTransferObject>> GetBooksAsync()
    {
        var baseUrl = $"http://localhost:5181/books";
        var result = await _httpClient.GetStringAsync(baseUrl);
        return JsonConvert.DeserializeObject<IEnumerable<BookDataTransferObject>>(result);
    }
}
