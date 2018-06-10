using System;

namespace Moneybox.App.Domain.Commands
{
    public interface IDepositCommand
    {
        void Execute(Guid accountId, decimal amount);
    }
}