using System.Data.SQLite;

class Transaction
{
    public int Id { get; set; }
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
    private const string ConnectionString = "Data Source=FinanceTracker.db;Version=3;";

    public FinanceTracker()
    {
        InitializeDatabase();
        transactions = LoadTransactionsFromDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();

            var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS Transactions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Description TEXT, Amount REAL, Date DATETIME)",
                connection);

            command.ExecuteNonQuery();
        }
    }

    private List<Transaction> LoadTransactionsFromDatabase()
    {
        var transactions = new List<Transaction>();

        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();

            var command = new SQLiteCommand("SELECT * FROM Transactions", connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    transactions.Add(new Transaction
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Description = Convert.ToString(reader["Description"]),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        Date = Convert.ToDateTime(reader["Date"])
                    });
                }
            }
        }

        return transactions;
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

        SaveTransactionToDatabase(transaction);

        Console.WriteLine("Transaction added successfully.");
    }

    private void SaveTransactionToDatabase(Transaction transaction)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();

            var command = new SQLiteCommand(
                "INSERT INTO Transactions (Description, Amount, Date) VALUES (@description, @amount, @date)",
                connection);

            command.Parameters.AddWithValue("@description", transaction.Description);
            command.Parameters.AddWithValue("@amount", transaction.Amount);
            command.Parameters.AddWithValue("@date", transaction.Date);

            command.ExecuteNonQuery();
        }
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

    public void ClearHistoryAndBalance()
    {
        transactions.Clear();

        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();

            var command = new SQLiteCommand("DELETE FROM Transactions", connection);
            command.ExecuteNonQuery();
        }
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
            Console.WriteLine("4. Clear History and Balance");
            Console.WriteLine("5. Exit");

            Console.Write("Enter your choice: ");
            string userInput = Console.ReadLine();

            if (int.TryParse(userInput, out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter description: ");
                        string description = Console.ReadLine();
                        Console.Write("Enter amount: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                        {
                            financeTracker.AddTransaction(description, amount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
                        }
                        break;
                    case 2:
                        financeTracker.DisplayTransactions();
                        Console.Clear(); // Clear the console after displaying transactions
                        break;
                    case 3:
                        decimal balance = financeTracker.CalculateBalance();
                        Console.WriteLine($"Current Balance: {balance:C}");
                        break;
                    case 4:
                        financeTracker.ClearHistoryAndBalance();
                        Console.WriteLine("Transaction history and balance cleared.");
                        Console.Clear(); // Clear the console after clearing history and balance
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }
}
