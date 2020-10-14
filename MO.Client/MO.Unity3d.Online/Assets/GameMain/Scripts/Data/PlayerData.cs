using MO.Protocol;
using MO.Unity3d.Entities;
using System.Collections.Generic;

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

        public float ServerX { get; set; }
        public float ServerY { get; set; }
        public float ServerZ { get; set; }
        public float ServerRX { get; set; }
        public float ServerRY { get; set; }
        public float ServerRZ { get; set; }
        public int KillCount { get; set; }
        public int DeadCount { get; set; }

        public int MaxBlood { get { return 1300; } }
        public int CurBlood { get; set; }
        public int EntityId { get { return (int)UserId * 100 + 1; } }
        public int HPEntityId { get { return (int)UserId * 100 + 2; } }
        public int SkillCEntityId { get { return (int)UserId * 100 + 5; } }
        public int SkillXEntityId { get { return (int)UserId * 100 + 6; } }
        public int SkillZEntityId { get { return (int)UserId * 100 + 7; } }
        public int SkillBigEntityId { get { return (int)UserId * 100 + 8; } }
        public byte JumpState { get; set; }

        public byte ResetState { get; set; }

        private bool ShowSkill(int entityId, float distance)
        {
            if (GameEntry.Entity.HasEntity(entityId))
                return false;

            var skillData = new SkillData();
            skillData.PlayerData = this;
            skillData.Distance = distance;
            GameEntry.Entity.ShowEntity<SkillEntity>(entityId,
                "Assets/GameMain/Entities/Skill.prefab", "DefaultEntityGroup", skillData);
            return true;
        }

        public void Jump()
        {
            JumpState = 1;
        }

        public bool ShowSkillC()
        {
            return ShowSkill(SkillCEntityId, 10);
        }

        public bool ShowSkillX()
        {
            return ShowSkill(SkillXEntityId, 6);
        }

        public bool ShowSkillZ()
        {
            return ShowSkill(SkillZEntityId, 3);
        }

        public bool ShowBigSkill()
        {
            if (GameEntry.Entity.HasEntity(SkillBigEntityId))
                return false;

            var skillData = new SkillData();
            skillData.PlayerData = this;
            GameEntry.Entity.ShowEntity<BigSkillEntity>(SkillBigEntityId,
                "Assets/GameMain/Entities/BigSkill.prefab", "DefaultEntityGroup", skillData);
            return true;
        }

        public void Reset()
        {
            ResetState = 1;
            CurBlood = MaxBlood;
            ServerX = 0;
            ServerY = 0;
            ServerZ = 0;
            ServerRX = 0;
            ServerRY = 0;
            ServerRZ = 0;
        }
    }
}
