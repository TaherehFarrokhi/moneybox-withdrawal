using System;

namespace Moneybox.App.Domain.Commands
{
    public interface IWithdrawCommand
    {
        void Execute(Guid accountId, decimal amount);
    }
}