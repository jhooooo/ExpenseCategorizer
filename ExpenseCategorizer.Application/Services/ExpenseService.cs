using ExpenseCategorizer.Domain;

namespace ExpenseCategorizer.Application
{
    public class ExpenseService : IExpenseService
    {
        private readonly IAIService _aiService;

        public ExpenseService(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<ExpenseAnalysis> CategorizeAsync(string expense)
        {
            //return await _aiService.CategorizeAsync(expense);

            return new ExpenseAnalysis() {
                Category = "Travel",
                SubCategory = "Airfare",
                Amount = 10,
                Merchant = "Delta Airlines",
                Confidence = 0.95m
            };
        }
    }
}
