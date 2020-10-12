using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Algorithm.Enum
{
    public enum BaseErrorType : uint
    {
        Success,
        /// <summary>
        /// 显示(错误)
        /// </summary>
        Shown,
        /// <summary>
        /// 隐藏(错误)
        /// </summary>
        Hidden
    }
}
