using GameFramework.DataTable;
using System;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
    public static class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}
