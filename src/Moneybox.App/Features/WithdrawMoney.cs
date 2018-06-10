using System;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Commands;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IWithdrawCommand _withdrawCommand;

        public WithdrawMoney(IWithdrawCommand withdrawCommand)
        {
            _withdrawCommand = withdrawCommand ?? throw new ArgumentNullException(nameof(withdrawCommand));
        }

        public void Execute(Guid accountId, decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

            _withdrawCommand.Execute(accountId, amount);
        }
    }
}
