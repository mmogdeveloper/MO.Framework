using MO.Algorithm.OnlineDemo;
using MO.GrainInterfaces.User;
using System;
using System.Numerics;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        public IUserGrain User { get; }
        public Vector3 Position { get; set; }
        public Vector3 Rotate { get; set; }

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

        public DateTime JumpTime { get; set; }
        public DateTime BigSkillTime { get; set; }
        public DateTime SkillCTime { get; set; }
        public DateTime SkillXTime { get; set; }
        public DateTime SkillZTime { get; set; }

        public void Reset()
        {
            _curBlood = MaxBlood;
            Position = new Vector3();
            Rotate = new Vector3();
        }

        public PlayerData(IUserGrain user)
        {
            User = user;
            _curBlood = MaxBlood;
            JumpTime = DateTime.MinValue;
            BigSkillTime = DateTime.MinValue;
            SkillCTime = DateTime.MinValue;
            SkillXTime = DateTime.MinValue;
            SkillZTime = DateTime.MinValue;
        }
    }
}
