using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.Data
{
    public class PlayerData
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }
        public int EntityId { get { return (int)UserId * 100 + 1; } }
        public int SkillCEntityId { get { return (int)UserId * 100 + 5; } }
        public int SkillXEntityId { get { return (int)UserId * 100 + 6; } }
        public int SkillZEntityId { get { return (int)UserId * 100 + 7; } }
        public int SkillBigEntityId { get { return (int)UserId * 100 + 8; } }
        public byte JumpState { get; set; }
    }
}
