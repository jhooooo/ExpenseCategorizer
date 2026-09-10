using ExpenseCategorizer.Domain;

namespace ExpenseCategorizer.Application
{
    public interface IAIService
    {
        public Task<ExpenseAnalysis> CategorizeAsync(ExpenseCategorizeRequest request);
    }
}
