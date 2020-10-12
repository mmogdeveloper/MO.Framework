using MO.Protocol;
using MO.Unity3d.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            TotalBlood = 1300;
            CurBlood = 1300;
        }
        public long UserId { get; set; }
        public string UserName { get; set; }

        public float ServerX { get; set; }
        public float ServerY { get; set; }
        public float ServerZ { get; set; }
        public float ServerRX { get; set; }
        public float ServerRY { get; set; }
        public float ServerRZ { get; set; }

        public int TotalBlood { get; }
        public int CurBlood { get; set; }
        public int EntityId { get { return (int)UserId * 100 + 1; } }
        public int HPEntityId { get { return (int)UserId * 100 + 2; } }
        public int SkillCEntityId { get { return (int)UserId * 100 + 5; } }
        public int SkillXEntityId { get { return (int)UserId * 100 + 6; } }
        public int SkillZEntityId { get { return (int)UserId * 100 + 7; } }
        public int SkillBigEntityId { get { return (int)UserId * 100 + 8; } }
        public byte JumpState { get; set; }

        private void ShowSkill(int entityId, float distance)
        {
            if (GameEntry.Entity.HasEntity(entityId))
                return;

            var skillData = new SkillData();
            skillData.PlayerData = this;
            skillData.Distance = distance;
            GameEntry.Entity.ShowEntity<SkillEntity>(entityId,
                "Assets/GameMain/Entities/Skill.prefab", "DefaultEntityGroup", skillData);
        }

        public void Jump()
        {
            JumpState = 1;
        }

        public void ShowSkillC()
        {
            ShowSkill(SkillCEntityId, 9);
        }

        public void ShowSkillX()
        {
            ShowSkill(SkillXEntityId, 6);
        }

        public void ShowSkillZ()
        {
            ShowSkill(SkillZEntityId, 3);
        }

        public void ShowBigSkill()
        {
            if (GameEntry.Entity.HasEntity(SkillBigEntityId))
                return;

            var skillData = new SkillData();
            skillData.PlayerData = this;
            GameEntry.Entity.ShowEntity<BigSkillEntity>(SkillBigEntityId,
                "Assets/GameMain/Entities/BigSkill.prefab", "DefaultEntityGroup", skillData);
        }
    }
}
