# BrokeNoMore

Lightweight student budget planner app.

## Architecture

- **Models**: Transaction, Budget
- **Services**: DataService (JSON file storage)
- **Components**: Home.razor (main dashboard)

## Data Storage

JSON files stored in `Data/` directory:
- `transactions.json`
- `budgets.json`
- `categories.json`

## Default Categories

Grocery, Restaurant, Rent, Health expenses, Salary, Inheritance, Gift

### Models

**Transaction**

```json5
{
	"id": 202501011800245, // <Required> 
	"timestamp": "",  // <Required>
	"type": "expense", // <Required>, expense|income
	"amount": 100.00,  // <Required>, positive if type=income|negative if type=expense
	"currency": "EUR",  // <Required>
	"description": "market",  // <Optional>
	"category": "grocery",  // <Optional>
	"budgetId": 1  // <Optional>
}
```

**Budget**

```json5
{
	"id": 1,  // <Required>
	"name": "April25", // <Required>
	"limit": 100.0,  // <Required>
	"currency": "EUR",  // <Required>
	"periodStartDate": "2025-01-01",  // <Required>
	"periodEndDate": "2025-02-01"  // <Required>
}
```