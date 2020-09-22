using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Algorithm.DDZ
{
    public class HandPokerComplieResult
    {
		public HandPokerType type { get; set; }
		/// <summary>
		/// 相同类型比较大小
		/// </summary>
		public int compareValue { get; set; }
		/// <summary>
		/// 牌
		/// </summary>
		public int[] value { get; set; }
		/// <summary>
		/// 牌面字符串
		/// </summary>
		public string[] text { get; set; }
	}
}
