using MO.GrainInterfaces.User;
using System;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        public IUser User { get; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }

        public bool IsMove { get; set; }
        public PlayerData(IUser user)
        {
            User = user;
        }

        public void SetLocation(float x, float y, float z,
            float rx, float ry, float rz)
        {
            X = x;
            Y = y;
            Z = z;

            RX = rx;
            RY = ry;
            RZ = rz;

            IsMove = true;
        }
    }
}
