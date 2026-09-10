using ExpenseCategorizer.Domain;

namespace ExpenseCategorizer.Application
{
    public interface IExpenseService
    {
        public Task<ExpenseAnalysis> CategorizeAsync(string request);
    }
}
