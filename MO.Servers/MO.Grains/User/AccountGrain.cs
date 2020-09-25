using MO.GrainInterfaces;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Transactions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.User
{
    public class AccountGrain : Grain, IAccount
    {
        private readonly ITransactionalState<Balance> _balance;

        public AccountGrain([TransactionalState("balance", StorageProviders.DefaultProviderName)] ITransactionalState<Balance> balance)
        {
            _balance = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public Task Deposit(ulong amount)
        {
            return _balance.PerformUpdate(x => x.Diamond += amount);
        }

        public Task Withdraw(ulong amount)
        {
            return _balance.PerformUpdate(x => x.Diamond -= amount);
        }

        public Task<ulong> GetBalance()
        {
            return _balance.PerformRead(x => x.Diamond);
        }
    }
}
