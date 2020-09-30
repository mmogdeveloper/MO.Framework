using MO.GrainInterfaces.Transaction;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.Transaction
{
    [StatelessWorker]
    public class ATMGrain : Grain, IATM
    {
        public Task Transfer<TBalance>(long fromAccount, long toAccount, ulong amountToTransfer) where TBalance : Balance
        {
            return Task.WhenAll(
                this.GrainFactory.GetGrain<IAccount<TBalance>>(fromAccount).Withdraw(amountToTransfer),
                this.GrainFactory.GetGrain<IAccount<TBalance>>(toAccount).Deposit(amountToTransfer));
        }
    }
}