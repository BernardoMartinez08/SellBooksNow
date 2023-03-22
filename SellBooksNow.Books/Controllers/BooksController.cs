using Microsoft.AspNetCore.Mvc;
using SellBooksNow.Books.Dtos;

namespace SellBooksNow.Books.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<BookDataTransferObject> Get()
        {
            return Database.Books;
        }
    }
}