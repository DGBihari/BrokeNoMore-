using System.Text.Json;
using BrokeNoMore.Models;

namespace BrokeNoMore.Services;

public class DataService
{
    private readonly string _dataPath;
    private readonly string _transactionsFile;
    private readonly string _budgetsFile;
    private readonly string _categoriesFile;

    public DataService()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        _transactionsFile = Path.Combine(_dataPath, "transactions.json");
        _budgetsFile = Path.Combine(_dataPath, "budgets.json");
        _categoriesFile = Path.Combine(_dataPath, "categories.json");
        InitializeData();
    }

    private void InitializeData()
    {
        if (!Directory.Exists(_dataPath))
            Directory.CreateDirectory(_dataPath);

        if (!File.Exists(_categoriesFile))
        {
            var defaultCategories = new List<string> { "Grocery", "Restaurant", "Rent", "Health expenses", "Salary", "Inheritance", "Gift" };
            SaveCategories(defaultCategories);
        }

        if (!File.Exists(_transactionsFile))
        {
            GenerateSeedTransactions();
        }

        if (!File.Exists(_budgetsFile))
        {
            GenerateSeedBudgets();
        }
    }

    private void GenerateSeedTransactions()
    {
        var transactions = new List<Transaction>();
        var random = new Random(42);
        var categories = GetCategories();
        var expenseCategories = categories.Take(4).ToList();
        var incomeCategories = categories.Skip(4).ToList();
        var startDate = DateTime.Now.AddMonths(-18);

        for (int i = 0; i < 1000; i++)
        {
            var isIncome = random.Next(100) < 20;
            var amount = isIncome ? random.Next(500, 3000) : random.Next(10, 500);
            var timestamp = startDate.AddHours(random.Next(0, 18 * 30 * 24));
            
            transactions.Add(new Transaction
            {
                Id = timestamp.Ticks,
                Timestamp = timestamp,
                Type = isIncome ? "income" : "expense",
                Amount = isIncome ? amount : -amount,
                Currency = "EUR",
                Category = isIncome ? incomeCategories[random.Next(incomeCategories.Count)] : expenseCategories[random.Next(expenseCategories.Count)],
                Description = isIncome ? "Income transaction" : "Expense transaction"
            });
        }

        SaveTransactions(transactions.OrderBy(t => t.Timestamp).ToList());
    }

    private void GenerateSeedBudgets()
    {
        var budgets = new List<Budget>();
        var startDate = DateTime.Now.AddMonths(-18);

        for (int i = 0; i < 18; i++)
        {
            var periodStart = startDate.AddMonths(i);
            budgets.Add(new Budget
            {
                Id = i + 1,
                Name = periodStart.ToString("MMM yyyy"),
                Limit = 2000,
                Currency = "EUR",
                PeriodStartDate = periodStart,
                PeriodEndDate = periodStart.AddMonths(1)
            });
        }

        SaveBudgets(budgets);
    }

    public List<Transaction> GetTransactions() => 
        JsonSerializer.Deserialize<List<Transaction>>(File.ReadAllText(_transactionsFile)) ?? new();

    public void SaveTransactions(List<Transaction> transactions) => 
        File.WriteAllText(_transactionsFile, JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true }));

    public List<Budget> GetBudgets() => 
        JsonSerializer.Deserialize<List<Budget>>(File.ReadAllText(_budgetsFile)) ?? new();

    public void SaveBudgets(List<Budget> budgets) => 
        File.WriteAllText(_budgetsFile, JsonSerializer.Serialize(budgets, new JsonSerializerOptions { WriteIndented = true }));

    public List<string> GetCategories() => 
        JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_categoriesFile)) ?? new();

    public void SaveCategories(List<string> categories) => 
        File.WriteAllText(_categoriesFile, JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true }));
}
