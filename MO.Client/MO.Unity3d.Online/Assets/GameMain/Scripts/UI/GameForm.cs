using MO.Unity3d.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.UI
{
    public class GameForm : UIFormLogic
    {
        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            //GameEntry.Entity.ShowEntity<PlayerEntity>(
            //    GameEntry.Entity.GenerateSerialId(),
            //    "Assets/GameMain/Entities/Self.prefab",
            //    "DefaultEntityGroup");
            //GameEntry.Entity.ShowEntity<PlayerEntity>(
            //    GameEntry.Entity.GenerateSerialId(),
            //    "Assets/GameMain/Entities/Self.prefab",
            //    "DefaultEntityGroup");
        }
    }
}
