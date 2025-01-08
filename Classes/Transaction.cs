using System;

namespace BankAccountApp.Classes
{
    public class Transaction
    {
        public int Id { get; set; } 
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } 
        public decimal Amount { get; set; } 

        public int BankAccountId { get; set; }
        public BankAccount? BankAccount { get; set; }

        public Transaction(string transactionId, DateTime date, string type, decimal amount)
        {
            TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId), "TransactionId cannot be null");
            Date = date;
            Type = type ?? throw new ArgumentNullException(nameof(type), "Type cannot be null");
            Amount = amount;
        }
    }
}