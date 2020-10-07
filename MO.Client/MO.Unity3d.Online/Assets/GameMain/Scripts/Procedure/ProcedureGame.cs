using GameFramework.Event;
using GameFramework.Procedure;
using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            GameEntry.Scene.LoadScene("Assets/GameMain/Scenes/Game.unity");
            _formId = GameEntry.UI.OpenUIForm("Assets/GameMain/UI/UIForms/GameForm.prefab", "DefaultUIGroup", procedureOwner);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Scene.UnloadScene("Assets/GameMain/Scenes/Game.unity");
            GameEntry.UI.CloseUIForm(_formId);
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
