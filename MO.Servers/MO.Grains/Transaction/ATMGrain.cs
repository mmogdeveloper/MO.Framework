using MO.GrainInterfaces.Transaction;
using MO.GrainInterfaces.User;
using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace MO.Grains.Transaction
{
    [StatelessWorker]
    public class ATMGrain : Grain, IATMGrain
    {
        public Task Transfer<TBalance>(IAccountGrain<TBalance> fromAccount, IAccountGrain<TBalance> toAccount, ulong amountToTransfer) where TBalance : Balance
        {
            return Task.WhenAll(
                fromAccount.Withdraw(amountToTransfer),
                toAccount.Deposit(amountToTransfer));
        }
    }
}