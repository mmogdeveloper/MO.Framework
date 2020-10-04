using GameFramework.Event;
using GameFramework.Network;
using GameFramework.Procedure;
using Google.Protobuf;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.Network;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace MO.Unity3d.Procedure
{
    public class ProcedureLogin : ProcedureBase
    {
        private int _formId;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            GameEntry.Scene.LoadScene("Assets/GameMain/Scenes/Login.unity", this);
            _formId = GameEntry.UI.OpenUIForm("Assets/GameMain/UI/UIForms/LoginForm.prefab", "DefaultUIGroup");
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            GameEntry.Scene.UnloadScene("Assets/GameMain/Scenes/Login.unity");
            GameEntry.UI.CloseUIForm(_formId);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!GlobalGame.IsGameStart)
                return;

            ChangeState<ProcedureGame>(procedureOwner);
        }

        private void OnWebRequestSuccess(object sender, GameEventArgs args)
        {
            var webargs = (WebRequestSuccessEventArgs)args;
            if (webargs.UserData == typeof(C2S1003))
            {
                var strResult = Encoding.UTF8.GetString(webargs.GetWebResponseBytes());
                var moResult = MOMsgResult.Parser.ParseFrom(ByteString.FromBase64(strResult));
                var rep1003 = S2C1003.Parser.ParseFrom(moResult.Content);
                var playerData = new PlayerData();
                playerData.UserId = rep1003.UserId;
                playerData.UserName = rep1003.UserName;

                GameUser.Instance.Initiation(playerData);
                GameUser.Instance.Token = rep1003.Token;
                Log.Info("{0}登录成功", GameUser.Instance.UserName);
                GameUser.Instance.Channel.Connect(IPAddress.Parse(rep1003.GateIP), rep1003.GatePort);
            }
        }

        private void OnWebRequestFailure(object sender, GameEventArgs args)
        {
            var webargs = (WebRequestFailureEventArgs)args;
            if (webargs.UserData == typeof(C2S1003))
            {
                Log.Info("{0}登录失败", GameUser.Instance.UserName);
            }
        }
    }
}
