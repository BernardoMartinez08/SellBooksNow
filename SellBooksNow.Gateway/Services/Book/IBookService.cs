using SellBooksNow.Gateway.Dtos;

namespace VirtualLibrary.Books.Services.Event;

public interface IBookService
{
    Task<IEnumerable<BookDataTransferObject>> GetBooksAsync();
}