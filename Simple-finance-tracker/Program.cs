using System;
using System.Collections.Generic;

class Transaction
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    public override string ToString()
    {
        return $"{Date:d} - {Description}: {Amount:C}";
    }
}

class FinanceTracker
{
    private List<Transaction> transactions;

    public FinanceTracker()
    {
        transactions = new List<Transaction>();
    }

    public void AddTransaction(string description, decimal amount)
    {
        Transaction transaction = new Transaction
        {
            Description = description,
            Amount = amount,
            Date = DateTime.Now
        };

        transactions.Add(transaction);
        Console.WriteLine("Transaction added successfully.");
    }

    public void DisplayTransactions()
    {
        Console.WriteLine("Transaction History:");
        foreach (var transaction in transactions)
        {
            Console.WriteLine(transaction);
        }
    }

    public decimal CalculateBalance()
    {
        decimal balance = 0;
        foreach (var transaction in transactions)
        {
            balance += transaction.Amount;
        }
        return balance;
    }
}

class Program
{
    static void Main()
    {
        FinanceTracker financeTracker = new FinanceTracker();

        while (true)
        {
            Console.WriteLine("1. Add Transaction");
            Console.WriteLine("2. Display Transactions");
            Console.WriteLine("3. Calculate Balance");
            Console.WriteLine("4. Exit");

            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Enter description: ");
                    string description = Console.ReadLine();
                    Console.Write("Enter amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());
                    financeTracker.AddTransaction(description, amount);
                    break;
                case 2:
                    financeTracker.DisplayTransactions();
                    break;
                case 3:
                    decimal balance = financeTracker.CalculateBalance();
                    Console.WriteLine($"Current Balance: {balance:C}");
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
