using MO.Algorithm.OnlineDemo;
using MO.Unity3d.Data;
using MO.Unity3d.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MO.Unity3d.UIExtension
{
    public static class EntityExtension
    {
        private static bool ShowSkill(PlayerData playerData, int entityId, float distance)
        {
            if (GameEntry.Entity.HasEntity(entityId))
                return false;

            var skillData = new SkillData();
            skillData.PlayerData = playerData;
            skillData.Distance = distance;
            GameEntry.Entity.ShowEntity<SkillEntity>(entityId,
                "Assets/GameMain/Entities/Skill.prefab", "DefaultEntityGroup", skillData);
            return true;
        }

        public static bool ShowSkillC(this PlayerData playerData)
        {
            return ShowSkill(playerData, playerData.SkillCEntityId, DemoValue.SkillCDistance);
        }

        public static bool ShowSkillX(this PlayerData playerData)
        {
            return ShowSkill(playerData, playerData.SkillXEntityId, DemoValue.SkillXDistance);
        }

        public static bool ShowSkillZ(this PlayerData playerData)
        {
            return ShowSkill(playerData, playerData.SkillZEntityId, DemoValue.SkillZDistance);
        }

        public static bool ShowBigSkill(this PlayerData playerData)
        {
            if (GameEntry.Entity.HasEntity(playerData.SkillBigEntityId))
                return false;

            var skillData = new SkillData();
            skillData.PlayerData = playerData;
            GameEntry.Entity.ShowEntity<BigSkillEntity>(playerData.SkillBigEntityId,
                "Assets/GameMain/Entities/BigSkill.prefab", "DefaultEntityGroup", skillData);
            return true;
        }

        public static bool ShowEntity(this PlayerData playerData)
        {
            if (GameEntry.Entity.HasEntity(playerData.EntityId))
                return false;

            GameEntry.Entity.ShowEntity<PlayerEntity>(playerData.EntityId,
                            "Assets/GameMain/Entities/Player.prefab", "DefaultEntityGroup", playerData);
            return true;
        }

        public static bool ShowHP(this PlayerData playerData)
        {
            if (GameEntry.Entity.HasEntity(playerData.HPEntityId))
                return false;

            GameEntry.Entity.ShowEntity<HPBarEntity>(playerData.HPEntityId,
                "Assets/GameMain/Entities/HP_Bar.prefab", "DefaultEntityGroup", playerData);
            return true;
        }
    }
}
