using BankAccountApp.Classes;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Figgle;

namespace BankAccountApp
{
    internal class Program : MenuHelpers
    {
        static void Main(string[] args)
        {
            try
            {
                using var context = new BankDbContext();

                if (context.BankAccounts.Any())
                {
                    bool appIsRunning = true;
                    string title = "Bank Account App";
                    string asciiArt = FiggleFonts.Ivrit.Render(title);
  
                    while (appIsRunning)
                    {
                        ClearConsoleShowHeadline(asciiArt);
                        string menuOptionChoosed = PrintOutUserMenu();
                        ClearConsoleShowHeadline(asciiArt);

                        switch (menuOptionChoosed)
                        {
                            case "List all accounts":
                                AccountOperations.ListAllAccounts(context);
                                break;
                            case "Deposit money":
                                AccountOperations.DepositMoney(context);
                                break;
                            case "Withdraw money":
                                AccountOperations.WithdrawMoney(context);
                                break;
                            case "Transfer money":
                                AccountOperations.TransferMoney(context);
                                break;
                            case "Check account balance":
                                AccountOperations.CheckBalance(context);
                                break;
                            case "Show transaction history":
                                AccountOperations.TransferHistory(context);
                                break;
                            case "Exit":
                                appIsRunning = false;
                                Console.WriteLine("Exiting application.");
                                break;
                            default:
                                Console.WriteLine("Invalid option, try again.");
                                break;
                        }
                        if (appIsRunning)
                        {
                            PromptContinue();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No accounts found in the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}