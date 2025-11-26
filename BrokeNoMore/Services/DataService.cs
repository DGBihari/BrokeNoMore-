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
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"); // dir: \BrokeNoMore\bin\Debug\net9.0
        _transactionsFile = Path.Combine(_dataPath, "transactions.json");
        _budgetsFile = Path.Combine(_dataPath, "budgets.json");
        _categoriesFile = Path.Combine(_dataPath, "categories.json");
        InitializeData();

        LinkBudgetsToTransactions(); // constructor
    }
    public void LinkBudgetsToTransactions()
    {
        var transactions = GetTransactions();
        var budgets = GetBudgets();

        foreach (var t in transactions)
        {
            // Find budget where transaction falls in the budget period
            var budget = budgets.FirstOrDefault(b =>
                t.Timestamp >= b.PeriodStartDate && t.Timestamp <= b.PeriodEndDate
            );

            t.BudgetId = budget?.Id ?? 0; // Set budget ID, or 0 if none found (shouldn't happen)
        }

        SaveTransactions(transactions);
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

        if (!File.Exists(_budgetsFile))
        {
            GenerateSeedBudgets();
        }

        if (!File.Exists(_transactionsFile))
        {
            GenerateSeedTransactions();
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
        var budgets = GetBudgets();

        for (int i = 0; i < 1000; i++)
        {
            var isIncome = random.Next(100) < 20;
            var amount = isIncome ? random.Next(500, 3000) : random.Next(10, 500);
            var timestamp = startDate.AddHours(random.Next(0, 18 * 30 * 24));

            // Find the correct budget for this timestamp
           // var budget = budgets.FirstOrDefault(b => timestamp >= b.PeriodStartDate && timestamp < b.PeriodEndDate);

            transactions.Add(new Transaction
            {
                Id = timestamp.Ticks,
                Timestamp = timestamp,
                Type = isIncome ? "income" : "expense",
                Amount = isIncome ? amount : -amount,
                Currency = "EUR",
                Category = isIncome
                    ? incomeCategories[random.Next(incomeCategories.Count)]
                    : expenseCategories[random.Next(expenseCategories.Count)],
                Description = isIncome ? "Income transaction" : "Expense transaction",
                //BudgetId = budget?.Id ?? 0 // default 0 if no budget found
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
