using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.Transaction
{
    public interface IATM : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        Task Transfer(long fromAccount, long toAccount, ulong amountToTransfer);
    }
}
