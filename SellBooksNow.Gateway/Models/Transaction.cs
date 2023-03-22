namespace SellBooksNow.Gateway.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public string Status { get; set; }
    }

    public static class TransactionStatus
    {
        public const string InProcess = "En Proceso";
        public const string Charged = "Cobrado";
        public const string Completed = "Completed";
    }
}
