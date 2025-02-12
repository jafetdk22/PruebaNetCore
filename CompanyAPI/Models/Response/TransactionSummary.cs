namespace CompanyAPI.Models.Response
{
    public class TransactionSummary
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
