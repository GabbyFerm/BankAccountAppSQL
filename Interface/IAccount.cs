using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankAccountApp.Classes;

namespace BankAccountApp.Interface
{
    public interface IAccount
    {
        int AccountNumber { get; set; }
        decimal Balance { get; set; }
        string AccountType { get; set; }

        void Deposit(decimal amount);
        void Withdraw(decimal amount);
        void Transfer(decimal amount, IAccount targetAccount, IAccount accountTo);
        void CheckBalance();
    }
}