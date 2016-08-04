using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbcBank
{
    public class Customer
    {
        private String _name;
        private List<Account> _accounts;

        public Customer(String name)
        {
            this._name = name;
            this._accounts = new List<Account>();
        }

        public String Name
        {
            get { return _name; }
        }

        public Customer OpenAccount(Account account)
        {
            _accounts.Add(account);
            return this;
        }

        public int GetNumberOfAccounts()
        {
            return _accounts.Count;
        }

        public double TotalInterestEarned()
        {
            return _accounts.Select(a => a.InterestEarned()).Sum();
        }

        /*******************************
         * This method gets a statement 
         *********************************/
        public String GetStatement()
        {
            var statementBuilder = new StringBuilder();
            statementBuilder.AppendFormat("Statement for {0}{1}", _name, Environment.NewLine);

            double total = 0.0;
            foreach (Account a in _accounts)
            {
                statementBuilder.AppendFormat("{0}{1}{0}", Environment.NewLine, StatementForAccount(a));
                total += a.SumTransactions();
            }

            statementBuilder.AppendFormat("{0}Total In All Accounts {1}", Environment.NewLine, ToDollars(total));
     
            return statementBuilder.ToString();
        }

        //Transfer between a customer's accounts
        public void TransferBetweenAccounts(Account fromAccount, Account toAccount, double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount must be greater than zero");
            }

            //Make sure customers can only transfer between their _accounts - remove this check if this is not a requirment
            if (_accounts.SingleOrDefault(act => act.AccountId == fromAccount.AccountId) == null
                || _accounts.SingleOrDefault(act => act.AccountId == toAccount.AccountId) == null)
            {
                throw new ArgumentException("You can only transfer money between your own accounts.");
            }

            if (amount > fromAccount.Balance)
            {
                throw new ArgumentException("The Amount cannot be larger than the balance of the account you are transfering from.");
            }

            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);
        }

        private String StatementForAccount(Account a)
        {
            var statementBuilder = new StringBuilder();
       
            switch (a.AccountType)
            {
                case AccountType.CHECKING:
                    statementBuilder.AppendLine("Checking Account");
                    break;
                case AccountType.SAVINGS:
                    statementBuilder.AppendLine("Savings Account");
                    break;
                case AccountType.MAXI_SAVINGS:
                    statementBuilder.AppendLine("Maxi Savings Account");
                    break;
            }

            //Now total up all the transactions
            double total = 0.0;
            foreach (Transaction t in a.Transactions)
            {
                statementBuilder.AppendFormat(" {0} {1}{2}", (t.Amount < 0 ? "Withdrawal" : "Deposit"),
                    ToDollars(t.Amount), Environment.NewLine);
   
                total += t.Amount;
            }
            statementBuilder.AppendFormat("Total {0}", ToDollars(total));

            return statementBuilder.ToString();
        }

        private String ToDollars(double d)
        {
            return String.Format("${0:N2}", Math.Abs(d));
        }
    }
}
