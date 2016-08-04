using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace AbcBank.Test
{
    /********************************************
     * Note: These are just dummy test cases. We will need to test all the senarios as much as possible in real world projects
     ***************************************/
    [TestFixture]
    public class AccountTest
    {
        private Account _checkingAccount;
        private Account _savingsAccount;
        private Account _maxAccount;
        public const double INTEREST_DELTA = 0.0000001;

        [SetUp]
        public void Init()
        {
            _checkingAccount = new Account(AccountType.CHECKING);
            _savingsAccount = new Account(AccountType.SAVINGS);
            _maxAccount = new Account(AccountType.MAXI_SAVINGS);
        }

        [Test]
        public void ShouldCreateAccountBeSuccessful()
        {
            Assert.IsNotNull(_checkingAccount);
            Assert.IsNotNull(_savingsAccount);
            Assert.IsNotNull(_maxAccount);

            //Make sure AccountId is unique. n Real world, you should check Account Id's valid.
            var accounts = new List<Account> {_checkingAccount, _savingsAccount, _maxAccount};
            Assert.AreEqual(3, accounts.Select(a => a.AccountId).Distinct().ToList().Count);   
        }

        [Test]
        public void ShouldDepositBeSuccessful()
        {
            const double depositAmount = 100;
            _checkingAccount.Deposit(100);
            Assert.AreEqual(depositAmount, _checkingAccount.Transactions[0].Amount, double.Epsilon);

            _checkingAccount.Deposit(100);
            Assert.AreEqual(depositAmount, _checkingAccount.Transactions[0].Amount, double.Epsilon);

            _checkingAccount.Deposit(100);
            Assert.AreEqual(depositAmount, _checkingAccount.Transactions[0].Amount, double.Epsilon);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Amount must be greater than zero")]
        public void ShouldDepositThrowsErrorWhenAmountNotLargerThanZero()
        {
            _checkingAccount.Deposit(-1.0);
        }

        [Test]
        public void ShouldWithdrawBeSuccessful()
        {
            _checkingAccount.Deposit(100);
            _checkingAccount.Withdraw(50);
            Assert.AreEqual(50, _checkingAccount.Balance, double.Epsilon);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Amount must be greater than zero")]
        public void ShouldWithdrawThrowsErrorWhenAmountNotLargerThanZero()
        {
            _checkingAccount.Deposit(100);
            _checkingAccount.Withdraw(-1.0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Not enough money to withdraw")]
        public void ShouldWithdrawThrowsErrorIfAmountLargerThanBalance()
        {
            _savingsAccount.Deposit(100);
            _savingsAccount.Withdraw(150);
        }


        [Test]
        public void ShouldSumTransactionsBeSuccessful()
        {
            _maxAccount.Deposit(100);
            _maxAccount.Withdraw(50);
            _maxAccount.Deposit(10);
            Assert.AreEqual(60, _maxAccount.SumTransactions(), double.Epsilon);
        }

        [Test]
        public void ShouldInterestEarnedForCheckingBeCorrect()
        {
            //Mock a transaction
           
            DateTime now = DateTime.Now;
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.SetupGet(t => t.Amount).Returns(1000);
            mockTransaction.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-2));

            var checkingAccount = new Account(AccountType.CHECKING);
            checkingAccount.Transactions.Add(mockTransaction.Object);

            Assert.AreEqual(0.00547946, checkingAccount.InterestEarned(), INTEREST_DELTA);
        }

        [Test]
        public void ShouldInterestEarnedForSavingsLessThanOrEqualTo1000BeCorrect()
        {
            DateTime now = DateTime.Now;
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.SetupGet(t => t.Amount).Returns(1000);
            mockTransaction.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-2));

            var savingsAccount = new Account(AccountType.SAVINGS);
            savingsAccount.Transactions.Add(mockTransaction.Object);

            Assert.AreEqual(0.00547946, savingsAccount.InterestEarned(), INTEREST_DELTA);

        }

        [Test]
        public void ShouldInterestEarnedForSavingsMoreThan1000BeCorrect()
        {
            DateTime now = DateTime.Now;
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.SetupGet(t => t.Amount).Returns(2000);
            mockTransaction.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-2));

            var savingsAccount = new Account(AccountType.SAVINGS);
            savingsAccount.Transactions.Add(mockTransaction.Object);

            Assert.AreEqual(0.01643840, savingsAccount.InterestEarned(), INTEREST_DELTA);
        }

        [Test]
        public void ShouldInterestEarnedForMaxiSavingsWithoutWithdrawalInLastTenDaysBeCorrect()
        {
            DateTime now = DateTime.Now;
            var mockTransaction1 = new Mock<ITransaction>();
            mockTransaction1.SetupGet(t => t.Amount).Returns(2000);
            mockTransaction1.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-5));

            var mockTransaction2 = new Mock<ITransaction>();
            mockTransaction2.SetupGet(t => t.Amount).Returns(500);
            mockTransaction2.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-3));

            var maxAccount = new Account(AccountType.MAXI_SAVINGS);
            maxAccount.Transactions.Add(mockTransaction1.Object);
            maxAccount.Transactions.Add(mockTransaction2.Object);

            Assert.AreEqual(1.57574597, maxAccount.InterestEarned(), INTEREST_DELTA);
        }

        [Test]
        public void ShouldInterestEarnedForMaxiSavingsWithWithdrawlsInLastTenDaysBeCorrect()
        {
            DateTime now = DateTime.Now;
            var mockTransaction1 = new Mock<ITransaction>();
            mockTransaction1.SetupGet(t => t.Amount).Returns(2000);
            mockTransaction1.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-5));

            var mockTransaction2 = new Mock<ITransaction>();
            mockTransaction2.SetupGet(t => t.Amount).Returns(-500);
            mockTransaction2.SetupGet(t => t.TransactionDate).Returns(now.AddDays(-3));

            var maxAccount = new Account(AccountType.MAXI_SAVINGS);
            maxAccount.Transactions.Add(mockTransaction1.Object);
            maxAccount.Transactions.Add(mockTransaction2.Object);

            Assert.AreEqual(0.02328781, maxAccount.InterestEarned(), INTEREST_DELTA);
        }
    }
}
