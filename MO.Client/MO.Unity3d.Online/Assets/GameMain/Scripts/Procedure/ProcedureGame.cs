using GameFramework.Event;
using GameFramework.Procedure;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace MO.Unity3d.Procedure
{
    class ProcedureGame : ProcedureBase
    {
        private int _formId;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //// 卸载所有场景
            //string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            //for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            //{
            //    GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            //}

            //设置游戏场景摄像机视角
            Camera.main.transform.position = new Vector3(0, 10, -10);
            Vector3 eulerAngles = new Vector3(30, 0, 0);
            Camera.main.transform.Rotate(eulerAngles);

            GameEntry.Scene.LoadScene("Assets/GameMain/Scenes/Game.unity");
            _formId = GameEntry.UI.OpenUIForm("Assets/GameMain/UI/UIForms/GameForm.prefab", "DefaultUIGroup", procedureOwner);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Scene.UnloadScene("Assets/GameMain/Scenes/Game.unity");
            GameEntry.UI.CloseUIForm(_formId);
            GameEntry.Entity.HideAllLoadedEntities();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!GlobalGame.IsGameStart)
            {
                ChangeState<ProcedureLogin>(procedureOwner);
            }
        }
    }
}
