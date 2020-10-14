using MO.Algorithm.OnlineDemo;
using MO.GrainInterfaces.User;
using System;
using System.Numerics;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        public IUser User { get; }
        public Vector3 Position { get; private set; }
        public Vector3 Rotate { get; private set; }

        public int MaxBlood { get { return DemoValue.MaxBlood; } }

        public bool BloodChanged { get; set; }
        private int _curBlood;
        public int CurBlood
        {
            get { return _curBlood; }
            set
            {
                if (value < 0)
                    _curBlood = 0;
                else
                    _curBlood = value;
            }
        }

        public int KillCount { get; set; }
        public int DeadCount { get; set; }

        public void Reset()
        {
            _curBlood = MaxBlood;
            Position = new Vector3();
            Rotate = new Vector3();
        }

        public PlayerData(IUser user)
        {
            User = user;
            _curBlood = MaxBlood;
        }

        public void SetLocation(float x, float y, float z,
            float rx, float ry, float rz)
        {
            Position = new Vector3(x, y, z);
            Rotate = new Vector3(rx, ry, rz);
        }
    }
}
