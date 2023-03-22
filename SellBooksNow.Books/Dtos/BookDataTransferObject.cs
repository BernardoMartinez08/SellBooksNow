namespace SellBooksNow.Books.Dtos
{
    public class BookDataTransferObject
    {
        public string Isbn { get; set; }

        public string Title { get; set; }

        public int Pages { get; set; }

        public decimal Price { get; set; }

        public int NumberOfCopies { get; set; }
    }
}
