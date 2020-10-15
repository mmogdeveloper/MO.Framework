using MO.Protocol;
using MO.Unity3d.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.UI
{
    public class GameForm : UIFormLogic
    {
        private Text txtKillCount;
        private Text txtDeadCount;
        private Text txtOnlineCount;
        private Text txtRoomId;
        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            txtKillCount = GameObject.Find("txt_KillCount").GetComponent<Text>();
            txtDeadCount = GameObject.Find("txt_DeadCount").GetComponent<Text>();
            txtOnlineCount = GameObject.Find("txt_OnlineCount").GetComponent<Text>();
            txtRoomId = GameObject.Find("txt_RoomId").GetComponent<Text>();
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            txtKillCount.text = GameUser.Instance.CurPlayer.KillCount.ToString();
            txtDeadCount.text = GameUser.Instance.CurPlayer.DeadCount.ToString();
            txtOnlineCount.text = GameUser.Instance.Players.Count.ToString();
            txtRoomId.text = GameUser.Instance.RoomId.ToString();
        }

        public void OnExit()
        {
            C2S100005 content = new C2S100005();
            GlobalGame.SendPackage(content);
        }
    }
}
