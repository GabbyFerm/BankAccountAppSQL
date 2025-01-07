using BankAccountApp.Classes;
using BankAccountApp.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Spectre.Console;

namespace BankAccountApp.Classes
{
    public static class AccountOperations
    {
        public static void ListAllAccounts(BankDbContext context)
        {
            AnsiConsole.Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask("[lightseagreen]Loading accounts...[/]");

                while (!task.IsFinished)
                {
                    task.Increment(10);
                    Thread.Sleep(100);
                }

                task.Description = "[lightseagreen]Load complete![/]";
            });

            var table = new Table();

            table.AddColumn(new TableColumn("[bold lightseagreen]Account Type[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold lightseagreen]Account Number[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold lightseagreen]Balance[/]").LeftAligned());

            foreach (var account in context.BankAccounts)
            {
                table.AddRow(account.AccountType, account.AccountNumber.ToString(), $"{account.Balance:C}");
            }

            table.Border = TableBorder.Rounded;
            table.LeftAligned();
            table.BorderColor(Color.LightSeaGreen);
            AnsiConsole.Write(table);
        }
        public static void DepositMoney(BankDbContext context)
        {
            try
            {
                // Fetch bank account choices from the database
                var bankAccounts = context.BankAccounts
                    .Select(b => new AccountDisplay
                    {
                        AccountNumber = b.AccountNumber,
                        AccountType = b.AccountType,
                        DisplayText = $"{b.AccountType} - {b.AccountNumber}"
                    })
                    .ToList();

                // Use the interactive menu to choose an account
                var selectedAccountChoice = ChooseAccount(bankAccounts, "Choose an account to deposit money into:");

                if (selectedAccountChoice != null)
                {
                    Console.WriteLine($"You selected account number {selectedAccountChoice.AccountNumber}. Proceeding with deposit.");

                    // Retrieve the full account from the context
                    var account = context.BankAccounts.SingleOrDefault(b => b.AccountNumber == selectedAccountChoice.AccountNumber);

                    if (account != null)
                    {
                        // Ask for deposit amount
                        decimal depositAmount = AnsiConsole.Ask<decimal>("[lightseagreen]Enter the amount to deposit:[/]");

                        if (depositAmount <= 0)
                        {
                            AnsiConsole.MarkupLine("[red]Deposit amount must be greater than 0.[/]");
                            return;
                        }

                        // Simulate a progress bar
                        AnsiConsole.Progress()
                        .Start(ctx =>
                        {
                            var task = ctx.AddTask("[lightseagreen]Transaction in progress...[/]");

                            while (!task.IsFinished)
                            {
                                task.Increment(10);
                                Thread.Sleep(100);
                            }

                            task.Description = "[lightseagreen]Transaction completed![/]";
                        });

                        // Update the account balance
                        account.Balance += depositAmount;
                        context.SaveChanges(); // Save changes to the database

                        Console.WriteLine($"You have successfully deposited {depositAmount:C} into account {account.AccountType}. New balance: {account.Balance:C}.");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Account not found.[/]");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Account not found.[/]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static void WithdrawMoney(BankDbContext context)
        {
            var accountChoices = context.BankAccounts
                .Select(account => new
                {
                    account.AccountNumber,
                    account.AccountType,
                    DisplayText = $"{account.AccountType} - {account.AccountNumber}"
                })
                .ToList();

            // Use the DisplayText for user selection
            var selectedAccountWithdrawFrom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an account to withdraw money from:")
                    .PageSize(10)
                    .AddChoices(accountChoices.Select(account => account.DisplayText))
                    .HighlightStyle(new Style(Color.DarkTurquoise))
            );

            // Find the selected account by matching the DisplayText
            var selectedAccount = accountChoices
                .FirstOrDefault(account => account.DisplayText == selectedAccountWithdrawFrom);

            if (selectedAccount != null)
            {
                // Fetch the actual account object including the balance from the database
                var accountToWithdrawFrom = context.BankAccounts
                    .FirstOrDefault(account => account.AccountNumber == selectedAccount.AccountNumber);

                if (accountToWithdrawFrom != null)
                {
                    decimal withdrawalAmount = ValidateAmountInput(accountToWithdrawFrom.Balance);

                    AnsiConsole.Progress()
                        .Start(ctx =>
                        {
                            var task = ctx.AddTask("[lightseagreen]Transaction in progress...[/]");

                            while (!task.IsFinished)
                            {
                                task.Increment(10);
                                Thread.Sleep(100);
                            }

                            task.Description = "[lightseagreen]Transaction completed![/]";
                        });

                    // Check if the balance is sufficient
                    if (accountToWithdrawFrom.Balance >= withdrawalAmount)
                    {
                        accountToWithdrawFrom.Balance -= withdrawalAmount;
                        context.SaveChanges();

                        Console.WriteLine($"You have successfully withdrawn {withdrawalAmount:C} from account {accountToWithdrawFrom.AccountType}. New balance: {accountToWithdrawFrom.Balance:C}.");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Insufficient funds.[/]");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Account not found.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Account not found.[/]");
            }
        }
        public static void TransferMoney(BankDbContext context)
        {
            try
            {
                // Fetch bank accounts and their display information
                var accountChoices = context.BankAccounts
                    .Select(account => new
                    {
                        Account = account, // Keep reference to the actual BankAccount object
                        DisplayText = $"{account.AccountType} - {account.AccountNumber}" // Formatted display string
                    })
                    .ToList();  // Fetch data and format it in memory

                // Selecting account to withdraw from
                var selectedAccountWithdrawFrom = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an account to withdraw money from:")
                        .PageSize(10)
                        .AddChoices(accountChoices.Select(a => a.DisplayText).ToArray()) // Use DisplayText
                        .HighlightStyle(new Style(Color.DarkTurquoise))
                );

                var selectedAccountFrom = accountChoices
                    .FirstOrDefault(account => account.DisplayText == selectedAccountWithdrawFrom)?.Account;

                if (selectedAccountFrom == null)
                {
                    AnsiConsole.MarkupLine("[red]No account selected.[/]");
                    return;
                }

                // Validating the transfer amount
                decimal transferAmount = ValidateAmountInput(selectedAccountFrom.Balance);

                // Selecting account to transfer to
                var selectedAccountTransferTo = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose the account to transfer money to:")
                        .PageSize(10)
                        .AddChoices(accountChoices.Select(a => a.DisplayText).ToArray()) // Use DisplayText
                        .HighlightStyle(new Style(Color.DarkTurquoise))
                );

                var selectedAccountTo = accountChoices
                    .FirstOrDefault(account => account.DisplayText == selectedAccountTransferTo)?.Account;

                if (selectedAccountTo == null)
                {
                    AnsiConsole.MarkupLine("[red]No destination account selected.[/]");
                    return;
                }

                // Handling progress bar for the transaction
                AnsiConsole.Progress()
                    .Start(ctx =>
                    {
                        var task = ctx.AddTask("[lightseagreen]Transaction in progress...[/]");

                        while (!task.IsFinished)
                        {
                            task.Increment(10);
                            Thread.Sleep(100);
                        }

                        task.Description = "[lightseagreen]Transaction completed![/]";
                    });

                // Updating account balances
                selectedAccountFrom.Balance -= transferAmount;
                selectedAccountTo.Balance += transferAmount;

                // Creating transaction IDs and adding transaction records
                string transactionId = "TX" + DateTime.Now.Ticks.ToString();

                context.Transactions.Add(new Transaction(transactionId, DateTime.Now, "Transfer Out", transferAmount)
                {
                    BankAccountId = selectedAccountFrom.AccountNumber
                });

                context.Transactions.Add(new Transaction(transactionId, DateTime.Now, "Transfer In", transferAmount)
                {
                    BankAccountId = selectedAccountTo.AccountNumber
                });

                // Saving changes to the database
                context.SaveChanges();

                // Inform user of successful transfer
                Console.WriteLine($"You have successfully transferred {transferAmount:C} from account {selectedAccountFrom.AccountType} to {selectedAccountTo.AccountType}.");
            }
            catch (Exception ex)
            {
                // Log the error and show a message to the user
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public static void CheckBalance(BankDbContext context)
        {
            try
            {
                // Fetch bank accounts and their display information
                var accountChoices = context.BankAccounts
                    .Select(account => new
                    {
                        Account = account, // Keep reference to the actual BankAccount object
                        DisplayText = $"{account.AccountType} - {account.AccountNumber}" // Formatted display string
                    })
                    .ToList();  // Fetch data and format it in memory

                // Selecting account to check balance
                var selectedAccountChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an account to check balance:")
                        .PageSize(10)
                        .AddChoices(accountChoices.Select(a => a.DisplayText).ToArray()) // Use DisplayText
                        .HighlightStyle(new Style(Color.DarkTurquoise))
                );

                var selectedAccount = accountChoices
                    .FirstOrDefault(account => account.DisplayText == selectedAccountChoice)?.Account;

                if (selectedAccount != null)
                {
                    Console.WriteLine($"Account balance: {selectedAccount.Balance:C}");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Account not found.[/]");
                }
            }
            catch (Exception ex)
            {
                // Log the error and show a message to the user
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public static void TransferHistory(BankDbContext context)
        {
            // Retrieve the accounts and format DisplayText using AccountDisplay
            var accountChoices = context.BankAccounts
                                .Select(account => new AccountDisplay
                                {
                                    AccountType = account.AccountType,
                                    AccountNumber = account.AccountNumber,
                                    DisplayText = $"{account.AccountType} - {account.AccountNumber}"
                                })
                                .ToList(); // Execute the query and load the data into memory

            // Now use the formatted account choices for selection
            var selectedAccountCheckTransferHistory = ChooseAccount(accountChoices, "Choose an account to check transfer history:");

            if (selectedAccountCheckTransferHistory != null)
            {
                // Simulate a progress bar, you can tweak or remove this based on your needs
                AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("[lightseagreen]Loading account history...[/]");

                    while (!task.IsFinished)
                    {
                        task.Increment(10);
                        Thread.Sleep(100);
                    }

                    task.Description = "[lightseagreen]Load complete![/]";
                });

                // Retrieve the transactions for the selected account (ensure they are up-to-date)
                var transactions = context.Transactions
                    .Where(t => t.BankAccountId == selectedAccountCheckTransferHistory.AccountNumber)
                    .OrderByDescending(t => t.Date) // Optional: Sort by date (newest first)
                    .ToList(); // Fetch transactions

                Console.WriteLine($"Selected Account Number: {selectedAccountCheckTransferHistory.AccountNumber}");

                if (transactions.Any())
                {
                    // Display transactions for the selected account
                    foreach (var transaction in transactions)
                    {
                        Console.WriteLine($"Transaction: {transaction.Amount:C} | Date: {transaction.Date}");
                    }
                }
                else
                {
                    Console.WriteLine("[yellow]No transactions found for this account.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Account not found.[/]");
            }
        }

        public static AccountDisplay ChooseAccount(List<AccountDisplay> accountChoices, string promptMessage)
        {
            if (accountChoices == null || accountChoices.Count == 0)
            {
                throw new ArgumentException("No accounts available for selection.");
            }

            var choicePrompt = new SelectionPrompt<string>()
            .Title(promptMessage)
            .PageSize(10)
            .HighlightStyle(new Style(Color.DarkTurquoise));

            foreach (var account in accountChoices)
            {
                choicePrompt.AddChoices(account.DisplayText);
            }

            var selectedDisplayText = AnsiConsole.Prompt(choicePrompt);

            var selectedAccount = accountChoices.FirstOrDefault(a => a.DisplayText == selectedDisplayText);

            return selectedAccount ?? throw new InvalidOperationException("Account selection failed.");
        }

        public static decimal ValidateAmountInput(decimal balance)
        {
            while (true)
            {
                Console.Write("Enter amount: ");
                string amountInput = Console.ReadLine()!;

                if (decimal.TryParse(amountInput, out decimal amount))
                {
                    if (amount > balance)
                    {
                        AnsiConsole.MarkupLine("[red]You don't have that much money in the account.[/]");
                    }
                    else if (amount <= 0)
                    {
                        AnsiConsole.MarkupLine("[red]The amount must be greater than zero.[/]");
                    }
                    else
                    {
                        return amount;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid input. Please enter a valid number.[/]");
                }
            }
        }
    }
}