using GameFramework.Event;
using GameFramework.Procedure;
using Google.Protobuf;
using MO.Algorithm.OnlineDemo;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.UIExtension;
using UnityEngine;
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
			GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            //设置游戏场景摄像机视角
            Camera.main.transform.position = new Vector3(0, 10, -10);
            Vector3 eulerAngles = new Vector3(45, 0, 0);
            Camera.main.transform.eulerAngles = eulerAngles;

            GameEntry.Scene.LoadGameScene();
            _formId = GameEntry.UI.OpenGameForm();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
			GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Entity.HideAllLoadedEntities();
            GameEntry.UI.CloseUIForm(_formId);
            GameEntry.Scene.UnLoadGameScene();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
			if (GlobalGame.IsGameStart)
			{
				FrameUpdate();
			}
			else
			{
				ChangeState<ProcedureLogin>(procedureOwner);
			}
        }

		private void OnShowEntitySuccess(object sender, GameEventArgs args)
		{
			var successArgs = (ShowEntitySuccessEventArgs)args;
			if (successArgs.Entity.Id == GameUser.Instance.CurPlayer.EntityId)
			{
				transform = successArgs.Entity.transform;
			}
		}

		private Vector3 position;
        private Vector3 angles;
        private Transform transform;
	    private	void FrameUpdate()
		{
			if (transform == null)
				return;

			if (Vector3.Distance(position, transform.position) > 0.01f ||
				Vector3.Distance(angles, transform.eulerAngles) > 0.0004f)
			{
				position = transform.position;
				angles = transform.eulerAngles;

				var transformInfo = new TransformInfo()
				{
					Position = new MsgVector3()
					{
						X = position.x,
						Y = position.y,
						Z = position.z
					},
					Rotation = new MsgVector3()
					{
						X = angles.x,
						Y = angles.y,
						Z = angles.z
					}
				};
				var command = new CommandInfo();
				command.CommandId = (int)CommandType.Transform;
				command.CommandContent = transformInfo.ToByteString();
				GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
			}

			if (GameUser.Instance.CurPlayer.SendCommands.Count != 0)
			{
				var content = new C2S100009();
				while (GameUser.Instance.CurPlayer.SendCommands.Count != 0)
				{
					var command = GameUser.Instance.CurPlayer.SendCommands.Dequeue();
					content.Commands.Add(command);
				}
				GlobalGame.SendPackage(content);
			}
		}
	}
}
