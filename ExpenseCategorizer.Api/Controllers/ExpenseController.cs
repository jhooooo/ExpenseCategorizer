using ExpenseCategorizer.Application;
using ExpenseCategorizer.Domain;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseCategorizer.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost("categorize")]
        public async Task<ActionResult<ExpenseAnalysis>> Categorize(
            [FromBody] ExpenseCategorizeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return BadRequest("Expense description is required.");
            }

            var result = await _expenseService.CategorizeAsync(request.Description);

            return Ok(result);
        }

    }
}
