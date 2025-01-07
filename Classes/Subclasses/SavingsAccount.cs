namespace BankAccountApp.Classes.Subclasses
{
    public class SavingsAccount : BankAccount
    {
        public SavingsAccount(string accountType, int accountNumber, string userName, decimal balance, List<Transaction> transactions)
        : base(accountType, accountNumber, userName, balance)
        {
        }
    }
}