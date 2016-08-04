using System;
using Moq;
using NUnit.Framework;

namespace AbcBank.Test
{
    [TestFixture]
    public class CustomerTest
    {
        private Account _checkingAccount;
        private Account _savingsAccount;
        private Account _maxAccount;

        [SetUp]
        public void Init()
        {
            _checkingAccount = new Account(AccountType.CHECKING);
            _savingsAccount = new Account(AccountType.SAVINGS);
            _maxAccount = new Account(AccountType.MAXI_SAVINGS);
        }

        [Test] 
        public void ShouldGetCustomerStatementBeCorrect()
        {
            Customer henry = new Customer("Henry").OpenAccount(_checkingAccount).OpenAccount(_savingsAccount);

            _checkingAccount.Deposit(100.0);
            _savingsAccount.Deposit(4000.0);
            _savingsAccount.Withdraw(200.0);
     
            string expectedStatement =
                String.Format(
                    "Statement for Henry{0}{0}Checking Account{0} Deposit $100.00{0}Total $100.00{0}{0}Savings Account{0} Deposit $4,000.00{0} Withdrawal $200.00{0}Total $3,800.00{0}{0}Total In All Accounts $3,900.00",
                    Environment.NewLine);
            Assert.AreEqual(expectedStatement, henry.GetStatement());
        }

        [Test]
        public void ShouldGetNumberOfAccountBeCorrectForOneAccount()
        {     
            Customer oscar = new Customer("Oscar").OpenAccount(_savingsAccount);
            Assert.AreEqual(1, oscar.GetNumberOfAccounts());
        }

        [Test]
        public void ShouldGetNumberOfAccountBeCorrectForTwoAccounts()
        {
            Customer oscar = new Customer("Oscar")
                    .OpenAccount(_savingsAccount);
            oscar.OpenAccount(_checkingAccount);
            Assert.AreEqual(2, oscar.GetNumberOfAccounts());
        }

        [Test]
        public void ShouldGetNumberOfAccountBeCorrectForMultipleAccounts()
        {
            Customer oscar = new Customer("Oscar")
                    .OpenAccount(_savingsAccount);
            oscar.OpenAccount(_checkingAccount);
            oscar.OpenAccount(_maxAccount);
            Assert.AreEqual(3, oscar.GetNumberOfAccounts());
        }

        [Test]
        public void ShouldGetTotalInterestEarnedBeCorrect()
        {
            Customer oscar = new Customer("Oscar")
                .OpenAccount(_checkingAccount)
                .OpenAccount(_savingsAccount);
                
            //Mock transactions
            var mockTran1 = new Mock<ITransaction>();
            mockTran1.SetupGet(t => t.Amount).Returns(10000);
            mockTran1.SetupGet(t => t.TransactionDate).Returns(DateTime.Now.AddDays(-60));
            _checkingAccount.Transactions.Add(mockTran1.Object);

            var mockTran2 = new Mock<ITransaction>();
            mockTran2.SetupGet(t => t.Amount).Returns(10000);
            mockTran2.SetupGet(t => t.TransactionDate).Returns(DateTime.Now.AddDays(-30));
            _savingsAccount.Transactions.Add(mockTran2.Object);

            Assert.AreEqual(3.20573640, oscar.TotalInterestEarned(), AccountTest.INTEREST_DELTA);
        }

        [Test]
        public void ShouldTransferBetweenAccountsBeSuccessful()
        {
            _checkingAccount.Deposit(10000);
            _savingsAccount.Deposit(10000);

            Customer oscar = new Customer("Oscar")
               .OpenAccount(_checkingAccount)
               .OpenAccount(_savingsAccount);

            oscar.TransferBetweenAccounts(_checkingAccount, _savingsAccount, 500);
            Assert.AreEqual(9500, _checkingAccount.Balance, double.Epsilon);
            Assert.AreEqual(10500, _savingsAccount.Balance, double.Epsilon);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Amount must be greater than zero")]
        public void ShouldThrowErrorIfTransferAmountIsNotLargerThanZero()
        {
            _checkingAccount.Deposit(10000);
            _savingsAccount.Deposit(10000);

            Customer oscar = new Customer("Oscar")
               .OpenAccount(_checkingAccount)
               .OpenAccount(_savingsAccount);

            oscar.TransferBetweenAccounts(_checkingAccount, _savingsAccount, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "The Amount cannot be larger than the balance of the account you are transfering from.")]
        public void ShouldThrowErrorIfTransferAmountIsLargerThanFromAccount()
        {
            _checkingAccount.Deposit(10000);
            _savingsAccount.Deposit(10000);
     
            Customer oscar = new Customer("Oscar")
               .OpenAccount(_checkingAccount)
               .OpenAccount(_savingsAccount);

            oscar.TransferBetweenAccounts(_checkingAccount, _savingsAccount, 1000000);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "You can only transfer money between your own accounts.")]
        public void ShouldThrowErrorIfAttemptedToTransferNotWithinCustomerAccounts()
        {
            //Mock transactions
            _checkingAccount.Deposit(10000);
            _savingsAccount.Deposit(10000);

            var anotherCustomersAccunt = new Account(AccountType.CHECKING);
            anotherCustomersAccunt.Deposit(10000);

            Customer oscar = new Customer("Oscar")
               .OpenAccount(_checkingAccount)
               .OpenAccount(_savingsAccount);

            oscar.TransferBetweenAccounts(anotherCustomersAccunt, _savingsAccount, 100);
        }

    }
}
