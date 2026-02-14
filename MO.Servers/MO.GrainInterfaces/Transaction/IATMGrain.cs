using MO.GrainInterfaces.User;
using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Transaction
{
    public interface IATMGrain : IGrainWithIntegerKey
    {
        /// <summary>
        /// 转账
        /// </summary>
        [Orleans.TransactionAttribute(Orleans.TransactionOption.Create)]
        Task Transfer<TBalance>(IAccountGrain<TBalance> fromAccount, IAccountGrain<TBalance> toAccount, ulong amountToTransfer) where TBalance : Balance;
    }
}
