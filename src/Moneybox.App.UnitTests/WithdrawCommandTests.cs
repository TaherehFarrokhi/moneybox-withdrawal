using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Commands;
using Moneybox.App.Domain.Exceptions;
using Moneybox.App.Domain.Services;
using Moq;
using NUnit.Framework;

namespace Moneybox.App.UnitTests
{
    [TestFixture]
    public class WithdrawCommandTests
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<INotificationService> _notificationService;

        private WithdrawCommand _subject;

        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _notificationService = new Mock<INotificationService>();

            _subject = new WithdrawCommand(_accountRepository.Object, _notificationService.Object);
        }

        [TestCase(100)]
        public void Exceute_When_Account_Has_Not_Sufficient_Amount(decimal withdrawAmount)
        {
            // Arrange
            var account = new Account
            {
                Balance = withdrawAmount - .000000000000000001m
            };
            _accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(account);

            // Act

            // Assert
            Assert.Throws<InsufficientBalanceException>(() => _subject.Execute(Guid.NewGuid(), withdrawAmount));
        }

        [Test]
        public void Exceute_When_Account_Balance_And_Withrow_Are_Less_Than_Five_Hundreds()
        {
            // Arrange
            decimal withdrawAmount = new Random().Next(1, 500);
            var account = new Account
            {
                Balance = withdrawAmount,
                User = new User
                {
                    Email = "TestUser@TestDomain.com"
                }
            };
            _accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(account);
            var expectedBalance = 0;
            var expectedWithdraw = -withdrawAmount;
            // Act

            _subject.Execute(Guid.NewGuid(), withdrawAmount);
            // Assert
            _notificationService.Verify(m => m.NotifyFundsLow(It.Is<string>(x => x.Equals(account.User.Email))), Times.Once);
            _accountRepository.Verify(m => m.Update(It.Is<Account>(x =>
                x.User.Email.Equals(account.User.Email) && x.Balance == expectedBalance && x.Withdrawn == expectedWithdraw)));
        }

        [Test]
        public void Exceute_When_Account_Balance_Is_More_Than_Than_Five_Hundreds()
        {
            // Arrange
            var balance = new Random().Next(501, int.MaxValue);
            var withdrawAmount = balance - 1;
            var account = new Account
            {
                Balance = balance,
                User = new User
                {
                    Email = "TestUser@TestDomain.com"
                }
            };
            _accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(account);

            var expectedBalance = balance - withdrawAmount;
            var expectedWithdraw = -withdrawAmount;

            // Act

            _subject.Execute(Guid.NewGuid(), withdrawAmount);

            // Assert
            _notificationService.Verify(m => m.NotifyFundsLow(It.Is<string>(x => x.Equals(account.User.Email))),
                Times.Never);

            _accountRepository.Verify(m => m.Update(It.Is<Account>(x =>
                x.User.Email.Equals(account.User.Email) && x.Balance == expectedBalance &&
                x.Withdrawn == expectedWithdraw
            )));
        }

        [Test]
        public void Exceute_When_Account_IsNot_Exists_Throw_Exception()
        {
            // Arrange
            _accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns((Account)null);

            // Act

            // Assert
            Assert.Throws<AccountNotFoundException>(() => _subject.Execute(Guid.NewGuid(), 10m));
        }
    }
}
