using Orleans;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    [GenerateSerializer]
    public abstract class Balance
    {
        [Id(0)]
        public ulong Value { get; set; }
    }

    [GenerateSerializer]
    public class DiamondBalance : Balance { }

    [GenerateSerializer]
    public class ScoreBalance : Balance { }

    /// <summary>
    /// 房间内Balance账号Id (UserId&lt;&lt;24 + RoomId)
    /// </summary>
    /// <typeparam name="TBalance"></typeparam>
    public interface IAccountGrain<TBalance> : IGrainWithIntegerKey
        where TBalance : Balance
    {
        [Orleans.TransactionAttribute(Orleans.TransactionOption.CreateOrJoin)]
        Task Withdraw(ulong amount);

        [Orleans.TransactionAttribute(Orleans.TransactionOption.CreateOrJoin)]
        Task Deposit(ulong amount);

        [Orleans.TransactionAttribute(Orleans.TransactionOption.CreateOrJoin)]
        Task<ulong> GetBalance();

        [Orleans.TransactionAttribute(Orleans.TransactionOption.CreateOrJoin)]
        Task Clear();
    }
}
