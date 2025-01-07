using Spectre.Console;
using Figgle;

namespace BankAccountApp.Classes
{
    public class MenuHelpers
    {

        public static void ClearConsoleShowHeadline(string asciiArt)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[bold cyan]{asciiArt}[/]");
            AnsiConsole.MarkupLine($"[bold cyan]Welcome to your bank!\n[/]");
        }
        public static string PrintOutUserMenu()
        {
            var menuOptions = new string[]
            {
                "List all accounts",
                "Deposit money",
                "Withdraw money",
                "Transfer money",
                "Check account balance",
                "Show transaction history",
                "Exit"
            };

            var menuSelection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an option:")
                    .PageSize(10)
                    .AddChoices(menuOptions)
                    .HighlightStyle(new Style(Color.DarkTurquoise))
            );

            return menuSelection;
        }

        public static void PromptContinue()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}