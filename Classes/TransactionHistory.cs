using BankAccountApp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BankAccountApp.Classes
{
    public class TransactionHistory : ITransactionHistory
    {
        private readonly BankDbContext _dbContext;

        public TransactionHistory(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // This will fetch transactions from the database
        public IReadOnlyList<Transaction> Transactions
            => _dbContext.Transactions.Include(t => t.BankAccount).ToList();

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
        }

        public void ShowTransactionHistory()
        {
            var transactions = Transactions;

            foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction ID: {transaction.TransactionId}");
                Console.WriteLine($"Date: {transaction.Date}");
                Console.WriteLine($"Type: {transaction.Type}");
                Console.WriteLine($"Amount: {transaction.Amount}");
                Console.WriteLine($"Account: {transaction.BankAccount?.AccountNumber}");
                Console.WriteLine("---------------------------------------");
            }
        }
    }
}