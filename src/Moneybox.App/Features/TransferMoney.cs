using System;
using Moneybox.App.Domain.Commands;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IWithdrawCommand _withdrawCommand;
        private readonly IDepositCommand _depositCommand;

        public TransferMoney(IWithdrawCommand withdrawCommand, IDepositCommand depositCommand)
        {
            _withdrawCommand = withdrawCommand ?? throw new ArgumentNullException(nameof(withdrawCommand));
            _depositCommand = depositCommand ?? throw new ArgumentNullException(nameof(depositCommand));
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

            _withdrawCommand.Execute(fromAccountId, amount);
            _depositCommand.Execute(toAccountId, amount);
        }
    }
}
