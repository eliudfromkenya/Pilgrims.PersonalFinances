// Payoff Calculator JavaScript Functions
let payoffChart = null;

// Initialize payoff timeline chart
window.initializePayoffChart = function (canvasId, chartData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Destroy existing chart if it exists
    if (payoffChart) {
        payoffChart.destroy();
    }

    payoffChart = new Chart(ctx, {
        type: 'line',
        data: chartData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: 'index'
            },
            plugins: {
                title: {
                    display: true,
                    text: 'Debt Payoff Timeline',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                },
                legend: {
                    display: true,
                    position: 'top'
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    borderColor: '#3b82f6',
                    borderWidth: 1,
                    callbacks: {
                        label: function(context) {
                            return context.dataset.label + ': ' + 
                                   new Intl.NumberFormat('en-US', {
                                       style: 'currency',
                                       currency: 'USD'
                                   }).format(context.parsed.y);
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Timeline'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Amount ($)'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    },
                    ticks: {
                        callback: function(value) {
                            return new Intl.NumberFormat('en-US', {
                                style: 'currency',
                                currency: 'USD',
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            }).format(value);
                        }
                    }
                }
            },
            elements: {
                point: {
                    radius: 3,
                    hoverRadius: 6
                },
                line: {
                    tension: 0.4
                }
            },
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            }
        }
    });
};

// Initialize debt comparison chart
window.initializeDebtComparisonChart = function (canvasId, comparisonData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: comparisonData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Strategy Comparison',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                },
                legend: {
                    display: true,
                    position: 'top'
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    borderColor: '#3b82f6',
                    borderWidth: 1
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Strategy'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Amount ($)'
                    },
                    ticks: {
                        callback: function(value) {
                            return new Intl.NumberFormat('en-US', {
                                style: 'currency',
                                currency: 'USD',
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            }).format(value);
                        }
                    }
                }
            },
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            }
        }
    });
};

// Initialize amortization chart
window.initializeAmortizationChart = function (canvasId, amortizationData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    new Chart(ctx, {
        type: 'area',
        data: amortizationData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Payment Breakdown Over Time',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                },
                legend: {
                    display: true,
                    position: 'top'
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    borderColor: '#3b82f6',
                    borderWidth: 1,
                    callbacks: {
                        label: function(context) {
                            return context.dataset.label + ': ' + 
                                   new Intl.NumberFormat('en-US', {
                                       style: 'currency',
                                       currency: 'USD'
                                   }).format(context.parsed.y);
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Payment Number'
                    },
                    stacked: true
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Payment Amount ($)'
                    },
                    stacked: true,
                    ticks: {
                        callback: function(value) {
                            return new Intl.NumberFormat('en-US', {
                                style: 'currency',
                                currency: 'USD',
                                minimumFractionDigits: 0,
                                maximumFractionDigits: 0
                            }).format(value);
                        }
                    }
                }
            },
            elements: {
                point: {
                    radius: 0,
                    hoverRadius: 4
                }
            },
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            }
        }
    });
};

// Utility functions for payoff calculator
window.payoffCalculatorUtils = {
    // Format currency
    formatCurrency: function(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    },

    // Format percentage
    formatPercentage: function(rate) {
        return (rate * 100).toFixed(2) + '%';
    },

    // Calculate monthly payment for given principal, rate, and term
    calculateMonthlyPayment: function(principal, annualRate, months) {
        if (annualRate === 0) return principal / months;
        
        const monthlyRate = annualRate / 12;
        const payment = principal * (monthlyRate * Math.pow(1 + monthlyRate, months)) / 
                       (Math.pow(1 + monthlyRate, months) - 1);
        return payment;
    },

    // Calculate remaining balance after n payments
    calculateRemainingBalance: function(principal, annualRate, months, paymentsMade) {
        if (paymentsMade >= months) return 0;
        
        const monthlyRate = annualRate / 12;
        const monthlyPayment = this.calculateMonthlyPayment(principal, annualRate, months);
        
        let balance = principal;
        for (let i = 0; i < paymentsMade; i++) {
            const interestPayment = balance * monthlyRate;
            const principalPayment = monthlyPayment - interestPayment;
            balance -= principalPayment;
        }
        
        return Math.max(0, balance);
    },

    // Generate amortization schedule
    generateAmortizationSchedule: function(principal, annualRate, monthlyPayment) {
        const schedule = [];
        const monthlyRate = annualRate / 12;
        let balance = principal;
        let paymentNumber = 1;
        
        while (balance > 0.01 && paymentNumber <= 600) { // Max 50 years
            const interestPayment = balance * monthlyRate;
            const principalPayment = Math.min(monthlyPayment - interestPayment, balance);
            balance -= principalPayment;
            
            schedule.push({
                paymentNumber: paymentNumber,
                paymentAmount: monthlyPayment,
                principalAmount: principalPayment,
                interestAmount: interestPayment,
                remainingBalance: Math.max(0, balance)
            });
            
            paymentNumber++;
        }
        
        return schedule;
    },

    // Animate counter
    animateCounter: function(element, start, end, duration = 1000) {
        const startTime = performance.now();
        const difference = end - start;
        
        function updateCounter(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function
            const easeOutQuart = 1 - Math.pow(1 - progress, 4);
            const current = start + (difference * easeOutQuart);
            
            if (element.dataset.format === 'currency') {
                element.textContent = payoffCalculatorUtils.formatCurrency(current);
            } else if (element.dataset.format === 'percentage') {
                element.textContent = payoffCalculatorUtils.formatPercentage(current / 100);
            } else {
                element.textContent = Math.round(current).toLocaleString();
            }
            
            if (progress < 1) {
                requestAnimationFrame(updateCounter);
            }
        }
        
        requestAnimationFrame(updateCounter);
    },

    // Initialize counter animations
    initializeCounterAnimations: function() {
        const counters = document.querySelectorAll('[data-animate-counter]');
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.dataset.animated) {
                    const target = parseFloat(entry.target.dataset.target) || 0;
                    this.animateCounter(entry.target, 0, target);
                    entry.target.dataset.animated = 'true';
                }
            });
        }, { threshold: 0.5 });
        
        counters.forEach(counter => observer.observe(counter));
    },

    // Export data to CSV
    exportToCSV: function(data, filename) {
        const csv = this.convertToCSV(data);
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    },

    // Convert array of objects to CSV
    convertToCSV: function(data) {
        if (!data || data.length === 0) return '';
        
        const headers = Object.keys(data[0]);
        const csvContent = [
            headers.join(','),
            ...data.map(row => 
                headers.map(header => {
                    const value = row[header];
                    return typeof value === 'string' && value.includes(',') 
                        ? `"${value}"` 
                        : value;
                }).join(',')
            )
        ].join('\n');
        
        return csvContent;
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize counter animations
    payoffCalculatorUtils.initializeCounterAnimations();
    
    // Add smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
    
    // Add loading states for buttons
    document.querySelectorAll('button[data-loading]').forEach(button => {
        button.addEventListener('click', function() {
            const originalText = this.innerHTML;
            this.innerHTML = '<i class="fas fa-spinner fa-spin mr-2"></i>Calculating...';
            this.disabled = true;
            
            // Re-enable after 2 seconds (adjust based on actual calculation time)
            setTimeout(() => {
                this.innerHTML = originalText;
                this.disabled = false;
            }, 2000);
        });
    });
});

// Debt strategy comparison utilities
window.debtStrategyUtils = {
    // Calculate debt avalanche strategy
    calculateAvalanche: function(debts, extraPayment) {
        const sortedDebts = [...debts].sort((a, b) => b.interestRate - a.interestRate);
        return this.calculateStrategy(sortedDebts, extraPayment);
    },

    // Calculate debt snowball strategy
    calculateSnowball: function(debts, extraPayment) {
        const sortedDebts = [...debts].sort((a, b) => a.balance - b.balance);
        return this.calculateStrategy(sortedDebts, extraPayment);
    },

    // Calculate custom strategy
    calculateCustom: function(debts, extraPayment, priorityField = 'priority') {
        const sortedDebts = [...debts].sort((a, b) => a[priorityField] - b[priorityField]);
        return this.calculateStrategy(sortedDebts, extraPayment);
    },

    // Generic strategy calculation
    calculateStrategy: function(sortedDebts, extraPayment) {
        let totalMonths = 0;
        let totalInterest = 0;
        let totalPayments = 0;
        const payoffOrder = [];
        
        let remainingDebts = sortedDebts.map(debt => ({...debt}));
        let availableExtra = extraPayment;
        
        while (remainingDebts.length > 0) {
            totalMonths++;
            
            // Pay minimums on all debts
            remainingDebts.forEach(debt => {
                const interestPayment = debt.balance * (debt.interestRate / 12);
                const principalPayment = Math.min(debt.minimumPayment - interestPayment, debt.balance);
                
                debt.balance -= principalPayment;
                totalInterest += interestPayment;
                totalPayments += debt.minimumPayment;
            });
            
            // Apply extra payment to first debt
            if (availableExtra > 0 && remainingDebts.length > 0) {
                const targetDebt = remainingDebts[0];
                const extraApplied = Math.min(availableExtra, targetDebt.balance);
                targetDebt.balance -= extraApplied;
                totalPayments += extraApplied;
            }
            
            // Remove paid off debts
            const paidOffDebts = remainingDebts.filter(debt => debt.balance <= 0);
            paidOffDebts.forEach(debt => {
                payoffOrder.push({
                    name: debt.name,
                    month: totalMonths,
                    originalBalance: debt.originalBalance || debt.balance
                });
                availableExtra += debt.minimumPayment; // Snowball effect
            });
            
            remainingDebts = remainingDebts.filter(debt => debt.balance > 0);
            
            // Safety check to prevent infinite loops
            if (totalMonths > 600) break;
        }
        
        return {
            totalMonths,
            totalInterest,
            totalPayments,
            payoffOrder
        };
    }
};

// Print functionality
window.printPayoffResults = function() {
    const printContent = document.querySelector('.payoff-results-container');
    if (!printContent) return;
    
    const printWindow = window.open('', '_blank');
    printWindow.document.write(`
        <html>
            <head>
                <title>Debt Payoff Results</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    .no-print { display: none !important; }
                    table { width: 100%; border-collapse: collapse; margin: 20px 0; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    .header { text-align: center; margin-bottom: 30px; }
                    .summary { background-color: #f9f9f9; padding: 15px; margin: 20px 0; }
                    @media print {
                        body { margin: 0; }
                        .page-break { page-break-before: always; }
                    }
                </style>
            </head>
            <body>
                <div class="header">
                    <h1>Debt Payoff Calculator Results</h1>
                    <p>Generated on ${new Date().toLocaleDateString()}</p>
                </div>
                ${printContent.innerHTML}
            </body>
        </html>
    `);
    
    printWindow.document.close();
    printWindow.print();
};