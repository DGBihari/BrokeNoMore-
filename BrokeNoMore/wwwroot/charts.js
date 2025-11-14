let categoryChart, monthlyChart, yearlyChart;

window.initCharts = (categoryData, monthlyData, yearlyData) => {
    if (categoryChart) categoryChart.destroy();
    if (monthlyChart) monthlyChart.destroy();
    if (yearlyChart) yearlyChart.destroy();

    const chartConfig = (label, income, expense) => ({
        type: 'bar',
        data: {
            labels: ['Income', 'Expense'],
            datasets: [{
                label: label,
                data: [income, expense],
                backgroundColor: ['rgba(40, 167, 69, 0.7)', 'rgba(220, 53, 69, 0.7)'],
                borderColor: ['rgba(40, 167, 69, 1)', 'rgba(220, 53, 69, 1)'],
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: { beginAtZero: true }
            },
            plugins: {
                legend: { display: false }
            }
        }
    });

    const categoryCanvas = document.getElementById('categoryChart');
    const monthlyCanvas = document.getElementById('monthlyChart');
    const yearlyCanvas = document.getElementById('yearlyChart');

    if (categoryCanvas && monthlyCanvas && yearlyCanvas) {
        const categories = Object.keys(categoryData);
        const amounts = Object.values(categoryData);
        const colors = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40', '#FF6384'];
        
        categoryChart = new Chart(categoryCanvas, {
            type: 'pie',
            data: {
                labels: categories,
                datasets: [{
                    data: amounts,
                    backgroundColor: colors.slice(0, categories.length),
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' }
                }
            }
        });
        
        monthlyChart = new Chart(monthlyCanvas, chartConfig('Monthly', monthlyData.income, monthlyData.expense));
        yearlyChart = new Chart(yearlyCanvas, chartConfig('Yearly', yearlyData.income, yearlyData.expense));
    }
};
