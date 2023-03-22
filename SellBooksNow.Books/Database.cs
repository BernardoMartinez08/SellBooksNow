using SellBooksNow.Books.Dtos;

namespace SellBooksNow.Books
{
    public static class Database
    {
        public static IEnumerable<BookDataTransferObject> Books = new List<BookDataTransferObject>
        {
            new BookDataTransferObject
            {
                Isbn = "9781593275846",
                Title = "Eloquent JavaScript, Second Edition",
                Pages = 472,
                NumberOfCopies = 3,
                Price = 200
            },
            new BookDataTransferObject
            {
                Isbn = "9781449331818",
                Title = "Learning JavaScript Design Patterns",
                Pages = 254,
                NumberOfCopies = 13,
                Price = 400
            },
            new BookDataTransferObject
            {
                Isbn = "9781449365035",
                Title = "Speaking JavaScript",
                Pages = 460,
                NumberOfCopies = 9,
                Price = 100
            },
            new BookDataTransferObject
            {
                Isbn = "9781491950296",
                Title = "Programming JavaScript Applications",
                Pages = 254,
                NumberOfCopies = 20,
                Price = 1250
            },
            new BookDataTransferObject
            {
                Isbn = "9781593277574",
                Title = "Understanding ECMAScript 6",
                Pages = 352,
                NumberOfCopies = 23,
                Price = 90
            },
            new BookDataTransferObject
            {
                Isbn = "9781491904244",
                Title = "You Don't Know JS",
                Pages = 255,
                NumberOfCopies = 90,
                Price = 50
            }
        };
    }
}
