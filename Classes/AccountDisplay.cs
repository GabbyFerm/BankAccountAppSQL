using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountApp.Classes
{
    public class AccountDisplay
    {
        public int AccountNumber { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}