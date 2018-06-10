using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Exceptions;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Domain.Commands
{
    public class WithdrawCommand : IWithdrawCommand
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawCommand(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void Execute(Guid accountId, decimal amount)
        {
            var account = _accountRepository.GetAccountById(accountId);
            if (account == null)
            {
                throw new AccountNotFoundException($"Account not found. AccountId={accountId}");
            }

            var balance = account.Balance - amount;
            if (balance < 0m)
            {
                throw new InsufficientBalanceException($"Insufficient funds to make transfer. AccountId={accountId}");
            }

            if (account.Balance < 500m)
            {
                _notificationService.NotifyFundsLow(account.User.Email);
            }

            account.Balance = account.Balance - amount;
            account.Withdrawn = account.Withdrawn - amount;

            _accountRepository.Update(account);
        }
    }
}