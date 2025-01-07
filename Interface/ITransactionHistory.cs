using BankAccountApp.Classes;
using System.Collections.Generic;

namespace BankAccountApp.Interface
{
    public interface ITransactionHistory
    {
        IReadOnlyList<Transaction> Transactions { get; }
        void AddTransaction(Transaction transaction);
        void ShowTransactionHistory();
    }
}