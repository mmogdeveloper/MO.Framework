using Orleans;
using System;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    public class Balance
    {
        public ulong Diamond { get; set; }
    }

    public interface IAccount : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        Task Withdraw(ulong amount);

        [Transaction(TransactionOption.Join)]
        Task Deposit(ulong amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<ulong> GetBalance();
    }
}
