using MO.GrainInterfaces;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Transactions.Abstractions;
using System;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class AccountGrain<TBalance> : Grain, IAccountGrain<TBalance>
        where TBalance : Balance, new()
    {
        private readonly ITransactionalState<TBalance> _balance;

        public AccountGrain([TransactionalState("balance", StorageProviders.DefaultProviderName)] ITransactionalState<TBalance> balance)
        {
            _balance = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public Task Withdraw(ulong amount)
        {
            return _balance.PerformUpdate(m =>
            {
                if (m.Value < amount)
                    throw new Exception("余额不足");
                m.Value -= amount;
            });
        }

        public Task Deposit(ulong amount)
        {
            return _balance.PerformUpdate(m => m.Value += amount);
        }

        public Task<ulong> GetBalance()
        {
            return _balance.PerformRead(m => m.Value);
        }

        public Task Clear()
        {
            return _balance.PerformUpdate(m => m.Value = 0);
        }
    }
}
