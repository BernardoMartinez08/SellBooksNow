namespace SellBooksNow.Billing.Dtos;
public class PaymentMethodInformationDataTransferObject
{
    public Guid CardNumber { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Cvv { get; set; }
}