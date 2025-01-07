namespace BankAccountApp.Classes.Subclasses
{
    public class PersonalAccount : BankAccount
    {
        public PersonalAccount(string accountType, int accountNumber, string userName, decimal balance, List<Transaction> transactions)
        : base(accountType, accountNumber, userName, balance)
        {
        }
    }
}
