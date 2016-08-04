using System;
using System.Collections.Generic;
using System.Linq;

namespace AbcBank
{
    public enum AccountType
    {
        CHECKING = 0,
        SAVINGS = 1,
        MAXI_SAVINGS = 2
    }

    public class Account 
    {
        private readonly AccountType  _accountType;
        private readonly string _accountId;
        private readonly List<ITransaction> _transactions;
        private double _balance;

        //This static field contains a list of all bank account numbers that we can check when generating a unique new account Id
        private static List<string> _accountIdList = new List<string>();

        public Account(AccountType accountType)
        {
            this._accountId = GenerateRandomUniqueAccountNumber();
            this._accountType = accountType;
            this._transactions = new List<ITransaction>();
        }

        public string AccountId
        {
            get { return _accountId;  }
        }

        public AccountType AccountType
        {
            get { return _accountType;  }
        }

        /******************************************
         * Interest earned or penalties etc. should be taken into consideration for balance in real life.
         * But for this excercise, we will only consider deposits or withdrawls for simpliciy purpose
        ***************************************/
        public double Balance
        {
            get { return _balance; }
        }

        public List<ITransaction> Transactions
        {
            get { return _transactions;  }
        }

        public void Deposit(double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero");
            }

            _transactions.Add(new Transaction(amount));
            _balance += amount;
        }

        public void Withdraw(double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero");
            }

            //Check whether there is enough money to be withdrawn
            if (amount > _balance )
            {
                throw new ArgumentException("Not enough money to withdraw");
            }

            _transactions.Add(new Transaction(-amount));
            _balance -= amount;
        }

        public double InterestEarned()
        {
            var firstTransaction = _transactions.FirstOrDefault();

            if (firstTransaction == null)
                return 0.0;

            double totalInterest = 0.0;
            double amount = firstTransaction.Amount;  //Init the beginning amount
            double dailyInterest = 0.0;

            int daysToCaculateInterest = (DateTime.Now - firstTransaction.TransactionDate).Days;

            //Caculate interest for each day and adjust the amount accordingly
            for (int i = 1; i <= daysToCaculateInterest; i++)
            {
                DateTime date = firstTransaction.TransactionDate.Date.AddDays(i);
                dailyInterest = GetDailyInterest(amount, date);
                totalInterest += dailyInterest;

                double transactionAmountOfTheDay =
                    _transactions.Where(t => t.TransactionDate.Date == date.Date).Select(t => t.Amount).Sum();

                amount = amount + dailyInterest + transactionAmountOfTheDay;
            }

            return totalInterest;
        }


        public double SumTransactions()
        {
            return _transactions.Select(t => t.Amount).Sum();
        }

        //Generate a unique new Id consisting of 10 digit numbers
        private string GenerateRandomUniqueAccountNumber()
        {
            while (true)
            {
                var random = new Random();
                string newId = string.Empty;
                for (int i = 0; i < 10; i++)
                {
                    newId = String.Format("{0}{1}", newId, random.Next(0, 10));
                }

                if (!_accountIdList.Contains(newId))
                {
                    _accountIdList.Add(newId);
                    return newId;
                }
            }
        }

        private double GetDailyInterest(double amount, DateTime date)
        {
            const int daysInYear = 365;
            switch (_accountType)
            {
                case AccountType.SAVINGS:
                    var first1000DailyRate = 0.001/daysInYear;
                    var after1000DailyRate = 0.002 / daysInYear;
                    if (amount <= 1000)
                        return amount * first1000DailyRate;

                    return 1000*first1000DailyRate + (amount - 1000)*after1000DailyRate;
                case AccountType.MAXI_SAVINGS:
                    var numOfWithdrawalsInLast10Days = _transactions.Count(t => t.Amount < 0 && t.TransactionDate.Date > date.AddDays(-10));

                    if (numOfWithdrawalsInLast10Days == 0)
                        return amount * 0.05/daysInYear;

                    return amount * 0.001/daysInYear;

                default:
                    return amount * 0.001/daysInYear;
            }
        }
    }
}
