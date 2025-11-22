using BrokeNoMore.Models;
using BrokeNoMore.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrokeNoMore.Components.Pages;

public partial class Home
{
    [Inject] private DataService _dataService { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    private List<Transaction> _transactions = new();
    private List<Budget> _budgets = new();
    private List<string> _categories = new();
    private int _currentPage = 1;
    private const int _pageSize = 10;
    private bool _showForm = false;
    private Transaction? _editingTransaction = null;
    private string _formType = "expense";
    private decimal _formAmount = 0;
    private string _formCategory = "";
    private string _formDescription = "";
    private DateTime _formTimestamp = DateTime.Now;
    private int? _formBudgetId = null;
    private bool _showBudgetForm = false;
    private Budget? _editingBudget = null;
    private string _budgetName = "";
    private decimal _budgetLimit = 0;
    private DateTime _budgetStart = DateTime.Now;
    private DateTime _budgetEnd = DateTime.Now.AddMonths(1);
    private string _searchId = "";
    private decimal _balance = new Random().Next(1000, 10001);
    private bool _showBudgets = false;
    private bool _showAddForms = false;


    protected override void OnInitialized()
    {
        LoadData();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var categoryData = GetSpendingByCategory();
            var monthly = GetMonthlyData();
            var yearly = GetYearlyData();
            await JS.InvokeVoidAsync("initCharts", 
                categoryData,
                new { income = monthly.income, expense = monthly.expense },
                new { income = yearly.income, expense = yearly.expense });
        }
    }

    private void LoadData()
    {
        _transactions = _dataService.GetTransactions();
        _budgets = _dataService.GetBudgets();
        _categories = _dataService.GetCategories();
    }

    // Transaction CRUD
    public void CreateTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
        _dataService.SaveTransactions(_transactions);
    }

    public List<Transaction> ReadTransactions() => _transactions;

    public List<Transaction> GetPagedTransactions()
    {
        var filtered = string.IsNullOrWhiteSpace(_searchId) 
            ? _transactions 
            : _transactions.Where(t => t.Id.ToString().Contains(_searchId)).ToList();
        return filtered.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
    }

    public int GetTotalPages()
    {
        var filtered = string.IsNullOrWhiteSpace(_searchId) 
            ? _transactions 
            : _transactions.Where(t => t.Id.ToString().Contains(_searchId)).ToList();
        return (int)Math.Ceiling(filtered.Count / (double)_pageSize);
    }

    private void SearchTransactions()
    {
        _currentPage = 1;
    }

    public void NextPage()
    {
        if (_currentPage < GetTotalPages()) _currentPage++;
    }

    public void PreviousPage()
    {
        if (_currentPage > 1) _currentPage--;
    }

    public void GoToPage(int page)
    {
        if (page >= 1 && page <= GetTotalPages()) _currentPage = page;
    }

    private void ShowAddForm()
    {
        _showAddForms = !_showAddForms;
        _showForm = true;
        _editingTransaction = null;
        _formType = "expense";
        _formAmount = 0;
        _formCategory = "";
        _formDescription = "";
        _formTimestamp = DateTime.Now;
    }

    private void EditTransaction(Transaction transaction)
    {
        _showForm = true;
        _editingTransaction = transaction;
        _formType = transaction.Type;
        _formAmount = Math.Abs(transaction.Amount);
        _formCategory = transaction.Category ?? "";
        _formDescription = transaction.Description ?? "";
        _formTimestamp = transaction.Timestamp;
        _formBudgetId = transaction.BudgetId;
    }

    private async Task SaveTransaction()
    {
        var transaction = new Transaction
        {
            Id = _formTimestamp.Ticks,
            Timestamp = _formTimestamp,
            Type = _formType,
            Amount = _formType == "income" ? _formAmount : -_formAmount,
            Currency = "EUR",
            Category = _formCategory,
            Description = _formDescription,
            BudgetId = _formBudgetId
        };

        if (_editingTransaction != null)
        {
            DeleteTransaction(_editingTransaction);
        }

        CreateTransaction(transaction);
        CancelForm();
        await RefreshCharts();
    }

    private void CancelForm()
    {
        _showForm = false;
        _editingTransaction = null;
    }

    private async Task DeleteTransactionConfirm(Transaction transaction)
    {
        DeleteTransaction(transaction);
        await RefreshCharts();
    }

    private async Task RefreshCharts()
    {
        var categoryData = GetSpendingByCategory();
        var monthly = GetMonthlyData();
        var yearly = GetYearlyData();
        await JS.InvokeVoidAsync("initCharts", categoryData,
            new { income = monthly.income, expense = monthly.expense },
            new { income = yearly.income, expense = yearly.expense });
    }

    public void UpdateTransaction(Transaction updated)
    {
        var index = _transactions.FindIndex(t => t.Id == updated.Id);
        if (index >= 0) _transactions[index] = updated;
        _dataService.SaveTransactions(_transactions);
    }

    public void DeleteTransaction(Transaction transaction)
    {
        _transactions.Remove(transaction);
        _dataService.SaveTransactions(_transactions);
    }

    // Budget CRUD
    public void CreateBudget(Budget budget)
    {
        budget.Id = _budgets.Any() ? _budgets.Max(b => b.Id) + 1 : 1;
        _budgets.Add(budget);
        _dataService.SaveBudgets(_budgets);
    }

    public List<Budget> ReadBudgets() => _budgets;

    public void UpdateBudget(Budget updated)
    {
        var index = _budgets.FindIndex(b => b.Id == updated.Id);
        if (index >= 0) _budgets[index] = updated;
        _dataService.SaveBudgets(_budgets);
    }

    public void DeleteBudget(int id)
    {
        _budgets.RemoveAll(b => b.Id == id);
        _dataService.SaveBudgets(_budgets);
    }

    // Category operations
    public void CreateCategory(string category)
    {
        if (!_categories.Contains(category))
        {
            _categories.Add(category);
            _dataService.SaveCategories(_categories);
        }
    }

    public void DeleteCategory(string category)
    {
        _categories.Remove(category);
        _dataService.SaveCategories(_categories);
    }

    public List<string> ReadCategories() => _categories;

    // Statistics
    public decimal GetTotalExpense(DateTime start, DateTime end) =>
        _transactions.Where(t => t.Type == "expense" && t.Timestamp >= start && t.Timestamp <= end)
            .Sum(t => Math.Abs(t.Amount));

    public decimal GetTotalIncome(DateTime start, DateTime end) =>
        _transactions.Where(t => t.Type == "income" && t.Timestamp >= start && t.Timestamp <= end)
            .Sum(t => t.Amount);

    public (decimal expense, decimal income) GetWeeklyData() =>
        (GetTotalExpense(DateTime.Now.AddDays(-7), DateTime.Now), GetTotalIncome(DateTime.Now.AddDays(-7), DateTime.Now));

    public (decimal expense, decimal income) GetMonthlyData() =>
        (GetTotalExpense(DateTime.Now.AddMonths(-1), DateTime.Now), GetTotalIncome(DateTime.Now.AddMonths(-1), DateTime.Now));

    public (decimal expense, decimal income) GetYearlyData() =>
        (GetTotalExpense(DateTime.Now.AddYears(-1), DateTime.Now), GetTotalIncome(DateTime.Now.AddYears(-1), DateTime.Now));

    public Dictionary<string, decimal> GetSpendingByCategory() =>
        _transactions.Where(t => t.Type == "expense" && t.Category != null)
            .GroupBy(t => t.Category)
            .ToDictionary(g => g.Key!, g => g.Sum(t => Math.Abs(t.Amount)));

    public string GetBudgetName(int? budgetId) =>
        budgetId.HasValue ? _budgets.FirstOrDefault(b => b.Id == budgetId.Value)?.Name ?? "N/A" : "N/A";

    private void ShowAddBudgetForm()
    {
        _showBudgets = !_showBudgets;
        _showBudgetForm = true;
        _editingBudget = null;
        _budgetName = "";
        _budgetLimit = 0;
        _budgetStart = DateTime.Now;
        _budgetEnd = DateTime.Now.AddMonths(1);
    }

    private void EditBudget(Budget budget)
    {
        _showBudgetForm = true;
        _editingBudget = budget;
        _budgetName = budget.Name;
        _budgetLimit = budget.Limit;
        _budgetStart = budget.PeriodStartDate;
        _budgetEnd = budget.PeriodEndDate;
    }

    private void SaveBudget()
    {
        var budget = new Budget
        {
            Name = _budgetName,
            Limit = _budgetLimit,
            Currency = "EUR",
            PeriodStartDate = _budgetStart,
            PeriodEndDate = _budgetEnd
        };

        if (_editingBudget != null)
        {
            budget.Id = _editingBudget.Id;
            UpdateBudget(budget);
        }
        else
        {
            CreateBudget(budget);
        }

        CancelBudgetForm();
    }

    private void CancelBudgetForm()
    {
        _showBudgetForm = false;
        _editingBudget = null;
    }

    private void DeleteBudgetConfirm(int id)
    {
        DeleteBudget(id);
    }
}
