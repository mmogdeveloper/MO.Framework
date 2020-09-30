using MO.GrainInterfaces.User;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Transaction
{
    public interface IATM : IGrainWithIntegerKey
    {
        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="toAccount"></param>
        /// <param name="amountToTransfer"></param>
        /// <returns></returns>
        [Transaction(TransactionOption.Create)]
        Task Transfer<TBalance>(long fromAccount, long toAccount, ulong amountToTransfer) where TBalance : Balance;
    }
}
