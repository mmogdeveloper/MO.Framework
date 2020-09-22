using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Algorithm.DDZ
{
    public enum HandPokerType
    {
        /// <summary>
        /// 个
        /// </summary>
        Single,
        /// <summary>
        /// 对
        /// </summary>
        Double,
        /// <summary>
        /// 三条
        /// </summary>
        Three,
        /// <summary>
        /// 三条带一个
        /// </summary>
        ThreeSingle,
        /// <summary>
        /// 三条带一对
        /// </summary>
        ThreeDouble,
        /// <summary>
        /// 顺子
        /// </summary>
        Straight,
        /// <summary>
        /// 连对
        /// </summary>
        StraightDouble,
        /// <summary>
        /// 飞机
        /// </summary>
        Airplane,
        /// <summary>
        /// 飞机带N个
        /// </summary>
        AirplaneSingle,
        /// <summary>
        /// 飞机带N对
        /// </summary>
        AirplaneDouble,
        /// <summary>
        /// 炸带二个
        /// </summary>
        BombSingle,
        /// <summary>
        /// 炸带二对
        /// </summary>
        BombDouble,
        /// <summary>
        /// 四条炸
        /// </summary>
        Bomb,
        /// <summary>
        /// 王炸
        /// </summary>
        KingBomb
    }
}
