using System.Data.SQLite;

// Represents a financial transaction
class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    // Overrides the ToString method to provide a custom string representation of the transaction
    public override string ToString()
    {
        return $"{Date:d} - {Description}: {Amount:C}";
    }
}

// Manages financial transactions and interacts with the SQLite database
class FinanceTracker
{
    private List<Transaction> transactions; // In-memory list to store transactions
    private const string ConnectionString = "Data Source=FinanceTracker.db;Version=3;"; // SQLite database connection string

    // Constructor initializes the database and loads transactions from it
    public FinanceTracker()
    {
        InitializeDatabase();
        transactions = LoadTransactionsFromDatabase();
    }

    // Initializes the SQLite database by creating a 'Transactions' table if it doesn't exist
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

    // Loads transactions from the SQLite database into the in-memory list
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

    // Adds a new transaction to the in-memory list and saves it to the database
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

    // Saves a transaction to the SQLite database
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

    // Displays all transactions in the console
    public void DisplayTransactions()
    {
        Console.WriteLine("Transaction History:");
        foreach (var transaction in transactions)
        {
            Console.WriteLine(transaction);
        }
    }

    // Calculates and returns the total balance based on all transactions
    public decimal CalculateBalance()
    {
        decimal balance = 0;
        foreach (var transaction in transactions)
        {
            balance += transaction.Amount;
        }
        return balance;
    }

    // Clears both the transaction history (in-memory list) and the database
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

// Main program entry point
class Program
{
    static void Main()
    {
        FinanceTracker financeTracker = new FinanceTracker();

        while (true)
        {
            // Display menu options to the user
            Console.WriteLine("1. Add Transaction");
            Console.WriteLine("2. Display Transactions");
            Console.WriteLine("3. Calculate Balance");
            Console.WriteLine("4. Clear History and Balance");
            Console.WriteLine("5. Exit");

            // Prompt the user for input
            Console.Write("Enter your choice: ");
            string userInput = Console.ReadLine();

            if (int.TryParse(userInput, out int choice))
            {
                // Process user's choice based on the selected menu option
                switch (choice)
                {
                    case 1:
                        // Add a new transaction
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
                        // Display transaction history
                        financeTracker.DisplayTransactions();
                        break;
                    case 3:
                        // Calculate and display the current balance
                        decimal balance = financeTracker.CalculateBalance();
                        Console.WriteLine($"Current Balance: {balance:C}");
                        break;
                    case 4:
                        // Clear transaction history and balance
                        financeTracker.ClearHistoryAndBalance();
                        Console.WriteLine("Transaction history and balance cleared.");
                        Console.Clear();
                        break;
                    case 5:
                        // Exit the program
                        Environment.Exit(0);
                        break;
                    default:
                        // Handle invalid user input
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                // Handle invalid user input
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }
}
