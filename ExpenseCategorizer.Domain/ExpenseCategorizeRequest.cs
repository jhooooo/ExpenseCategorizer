namespace ExpenseCategorizer.Domain
{
    public class ExpenseCategorizeRequest
    {
        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "SGD";
    }
}
