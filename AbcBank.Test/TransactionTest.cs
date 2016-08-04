using NUnit.Framework;

namespace AbcBank.Test
{
    [TestFixture]
    public class TransactionTest
    {
        [Test]
        public void ShouldTransactionBeCorrect()
        {
            Transaction t = new Transaction(5);
            Assert.AreEqual(true, t is Transaction);
            Assert.AreEqual(5, t.Amount);
        }
    }
}
