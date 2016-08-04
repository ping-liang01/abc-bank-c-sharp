using System;
using Moq;
using NUnit.Framework;

namespace AbcBank.Test
{
    [TestFixture]
    public class BankTest
    {
        private static readonly double DOUBLE_DELTA = 1e-15;

        [Test]
        public void customerSummary()
        {
            Bank bank = new Bank();
            Customer john = new Customer("John");
            john.OpenAccount(new Account(AccountType.CHECKING));
            bank.AddCustomer(john);

            Assert.AreEqual("Customer Summary\n - John (1 account)", bank.CustomerSummary());
        }

        [Test]
        public void ShouldTotalInterestPaidForCheckingAccountBeCorrect()
        {
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.SetupGet(t => t.Amount).Returns(3000.0);
            mockTransaction.SetupGet(t => t.TransactionDate).Returns(DateTime.Now.AddDays(-180));

            Bank bank = new Bank();
            Account checkingAccount = new Account(AccountType.CHECKING);
            checkingAccount.Transactions.Add(mockTransaction.Object);
            Customer bill = new Customer("Bill").OpenAccount(checkingAccount);
            bank.AddCustomer(bill);
            Assert.AreEqual(1.47981488, bank.TotalInterestPaid(), AccountTest.INTEREST_DELTA);
        }

        [Test]
        public void ShouldTotalInterestPaidForSavingAccountBeCorrect()
        {
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.SetupGet(t => t.Amount).Returns(3000.0);
            mockTransaction.SetupGet(t => t.TransactionDate).Returns(DateTime.Now.AddDays(-180));

            Bank bank = new Bank();
            Account savingAccount = new Account(AccountType.SAVINGS);
            savingAccount.Transactions.Add(mockTransaction.Object);
            Customer bill = new Customer("Bill").OpenAccount(savingAccount);
            bank.AddCustomer(bill);
            Assert.AreEqual(2.46696305, bank.TotalInterestPaid(), AccountTest.INTEREST_DELTA);
        }

        [Test]
        public void ShouldTotalInterestPaidForMaxAccountBeCorrect()
        {
            var mockTransaction = new Mock<ITransaction>();
            mockTransaction.SetupGet(t => t.Amount).Returns(3000.0);
            mockTransaction.SetupGet(t => t.TransactionDate).Returns(DateTime.Now.AddDays(-180));

            Bank bank = new Bank();
            Account savingAccount = new Account(AccountType.MAXI_SAVINGS);
            savingAccount.Transactions.Add(mockTransaction.Object);
            Customer bill = new Customer("Bill").OpenAccount(savingAccount);
            bank.AddCustomer(bill);;
            Assert.AreEqual(74.88694336, bank.TotalInterestPaid(), AccountTest.INTEREST_DELTA);
        }

    }
}
