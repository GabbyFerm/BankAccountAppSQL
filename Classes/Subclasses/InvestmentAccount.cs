namespace BankAccountApp.Classes.Subclasses
{
    public class InvestmentAccount : BankAccount
    {
        public InvestmentAccount(string accountType, int accountNumber, string userName, decimal balance, List<Transaction> transactions)
        : base(accountType, accountNumber, userName, balance)
        {
        }
    }
}
