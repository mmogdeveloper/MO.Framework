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
        public int EntityId { get; set; }
    }
}
