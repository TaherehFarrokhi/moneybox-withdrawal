using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Exceptions;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Domain.Commands
{
    public class DepositCommand : IDepositCommand
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public DepositCommand(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void Execute(Guid accountId, decimal amount)
        {
            var account = _accountRepository.GetAccountById(accountId);
            if (account == null) throw new AccountNotFoundException($"Account not found. AccountId={accountId}");

            var paidIn = account.PaidIn + amount;
            if (paidIn > Account.PayInLimit) throw new InvalidOperationException("Account pay in limit reached");

            if (Account.PayInLimit - paidIn < 500m)
                _notificationService.NotifyApproachingPayInLimit(account.User.Email);

            account.Balance += amount;
            account.PaidIn += amount;

            _accountRepository.Update(account);
        }
    }
}