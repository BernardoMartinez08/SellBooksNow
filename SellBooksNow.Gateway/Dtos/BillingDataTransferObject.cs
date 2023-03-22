using SellBooksNow.Gateway.Models;

namespace SellBooksNow.Gateway.Dtos
{
    public class BillingDataTransferObject
    {
        public BookInformationDataTransferObjet book_information { get; set; }
        public PaymentMethodInformationDataTransferObject payment_method { get; set; }
        public Transaction transaction { get; set; }
    }
}
