using System;

namespace AbcBank
{

    //Use interface so that transactions can ba mocked in unit tests
    public interface ITransaction
    {
        double Amount { get; }
        DateTime TransactionDate { get; }
    }

    public class Transaction : ITransaction
    {
        private readonly double _amount;
        private readonly DateTime _transactionDate;

        public Transaction(double amount)
        {
            this._amount = amount;
            this._transactionDate = DateProvider.GetInstance().Now();
        }

        public double Amount
        {
            get { return _amount;  }
        }

        public DateTime TransactionDate
        {
            get { return _transactionDate; }
        }
    }
}
