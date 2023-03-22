using SellBooksNow.Billing.Models;

namespace SellBooksNow.Billing.Dtos
{
    public class BillingDataTransferObject
    {
        public BookInformationDataTransferObjet book_information { get; set; }
        public PaymentMethodInformationDataTransferObject payment_method { get; set; }
        public Transaction transaction { get; set; }
    }
}
