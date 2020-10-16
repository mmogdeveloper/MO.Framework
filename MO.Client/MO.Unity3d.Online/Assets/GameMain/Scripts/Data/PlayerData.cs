using MO.Algorithm.OnlineDemo;
using MO.Protocol;
using MO.Unity3d.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace MO.Unity3d.Data
{
    public class PlayerData
    {
        public Queue<CommandInfo> SendCommands { get; }
        public Queue<CommandInfo> RecvCommands { get; }
        public PlayerData()
        {
            SendCommands = new Queue<CommandInfo>();
            RecvCommands = new Queue<CommandInfo>();
            CurBlood = MaxBlood;
        }
        public long UserId { get; set; }
        public string UserName { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotate { get; set; }

        public int KillCount { get; set; }
        public int DeadCount { get; set; }

        public int MaxBlood { get { return DemoValue.MaxBlood; } }
        public int CurBlood { get; set; }
        public int EntityId { get { return (int)UserId * 100 + 1; } }
        public int HPEntityId { get { return (int)UserId * 100 + 2; } }
        public int SkillCEntityId { get { return (int)UserId * 100 + 5; } }
        public int SkillXEntityId { get { return (int)UserId * 100 + 6; } }
        public int SkillZEntityId { get { return (int)UserId * 100 + 7; } }
        public int SkillBigEntityId { get { return (int)UserId * 100 + 8; } }

        public float PositionSpeed { get { return DemoValue.PositionSpeed; } }
        public float RotateSpeed { get { return DemoValue.RotateSpeed; } }

        //public Vector3 JumpPosition { get; private set; }
        public float JumpAnimationTime { get { return DemoValue.JumpAnimationTime; } }
        public float JumpDistance { get { return DemoValue.JumpDistance; } }

        public bool IsMoved { get; set; }
        public byte JumpState { get; set; }
        public byte ResetState { get; set; }
        public void Jump()
        {
            JumpState = 1;
        }

        public void Reset()
        {
            ResetState = 1;
            CurBlood = MaxBlood;
            Position = new Vector3();
            Rotate = new Vector3();
        }
    }
}
