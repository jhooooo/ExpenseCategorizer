namespace ExpenseCategorizer.Domain
{ 
    public class ExpenseAnalysis
    {
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Merchant { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
    }
}
