using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Algorithm.DDZ
{
    public class HandPokerInfo
    {
		/// <summary>
		/// 出牌时间
		/// </summary>
		public DateTime time { get; set; }
		/// <summary>
		/// 这手牌出自哪位玩家
		/// </summary>
		public int playerIndex { get; set; }
		/// <summary>
		/// 牌编译结果
		/// </summary>
		public HandPokerComplieResult result { get; set; }
	}
}
