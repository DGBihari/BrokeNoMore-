# BrokeNoMore

A simple budget planning app for students to track income and expenses.

## Overview

BrokeNoMore helps students manage their finances by tracking transactions, setting monthly budgets, and visualizing spending patterns. Built as a course project for Development of Financial IT Systems.

**Team:** Nihad Guluzade, Bihari Daniel Gergo

## Features

- Add, edit, and delete income/expense transactions
- Create monthly budgets with spending limits
- View spending breakdown by category
- Monthly and yearly financial overview charts
- Search and filter transactions

## Tech Stack

- Blazor Server (.NET 9.0)
- Bootstrap 5 for UI
- Chart.js for visualizations
- JSON file storage

## Getting Started

**Run the executable:**
1. Download from the releases section
2. Make sure you have .NET 9.0 installed
3. Run `BrokeNoMore.exe`
4. Open `http://localhost:5000/` in your browser

The app creates sample data automatically on first launch.

## Project Structure
```
BrokeNoMore/
├── Components/Pages/
│   ├── Home.razor          # Main UI
│   └── Home.razor.cs       # Business logic
├── Models/
│   ├── Transaction.cs
│   └── Budget.cs
├── Services/
│   └── DataService.cs      # Data persistence
└── Data/                   # JSON storage
    ├── transactions.json
    ├── budgets.json
    └── categories.json
```

## Data Models

**Transaction:**
```json
{
  "id": 202501011800245,
  "timestamp": "2025-01-01T18:00",
  "type": "expense",
  "amount": -100.00,
  "currency": "EUR",
  "category": "Grocery",
  "description": "Shopping",
  "budgetId": 1
}
```

**Budget:**
```json
{
  "id": 1,
  "name": "January 2025",
  "limit": 2000.00,
  "currency": "EUR",
  "periodStartDate": "2025-01-01",
  "periodEndDate": "2025-02-01"
}
```

**Default Categories:** Grocery, Restaurant, Rent, Health expenses, Salary, Inheritance, Gift

## Regulatory Compliance

Financial applications handle sensitive personal data, which makes regulatory compliance critical. BrokeNoMore was designed with GDPR principles in mind:

- **Data Minimization** - Only essential financial data collected
- **Local Storage** - All data stays on user's device
- **User Control** - Full rights to access, edit, and delete data
- **Transparency** - Human-readable JSON format
- **Privacy by Design** - No external services or tracking

See full compliance documentation in the project report.

## Privacy & Data Protection

- All data stored locally in JSON files
- No external services or data sharing
- Single-user application
- No cookies or tracking
- Users have complete control over their data

## Known Limitations

- Local storage only
- Desktop-optimized (limited mobile support)

## Future Enhancements

- Export to CSV/PDF
- Mobile app version (Blazor MAUI)
- Recurring transactions
- Budget alerts and notifications

## License

Academic project for educational purposes.
