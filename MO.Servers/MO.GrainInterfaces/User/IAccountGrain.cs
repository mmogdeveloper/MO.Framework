using Orleans;
using System;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    public abstract class Balance
    {
        public ulong Value { get; set; }
    }

    public class DiamondBalance : Balance { };
    public class ScoreBalance : Balance { };

    /// <summary>
    /// 房间内Balance账号Id (UserId<<24 + RoomId)
    /// </summary>
    /// <typeparam name="TBalance"></typeparam>
    public interface IAccountGrain<TBalance> : IGrainWithIntegerKey
        where TBalance : Balance
    {
        /// <summary>
        /// 取款
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Withdraw(ulong amount);

        /// <summary>
        /// 存款
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Deposit(ulong amount);

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<ulong> GetBalance();

        /// <summary>
        /// 清空账户
        /// </summary>
        /// <returns></returns>
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Clear();
    }
}
