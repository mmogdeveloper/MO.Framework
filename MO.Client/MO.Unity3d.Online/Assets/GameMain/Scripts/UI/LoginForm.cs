using GameFramework.Event;
using Google.Protobuf;
using LitJson;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.UI
{
    public class LoginForm : UIFormLogic
    {
        private InputField txtRoomId;
        private InputField txtUserName;

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            txtRoomId = GameObject.Find("txt_RoomId").GetComponent<InputField>();
            txtUserName = GameObject.Find("txt_UserName").GetComponent<InputField>();
        }

        public void OnLogin()
        {
            GameUser.Instance.RoomId = Int32.Parse(txtRoomId.text);
            C2S1003 content = new C2S1003();
            content.DeviceId = txtUserName.text;
            content.MobileType = 1;
            var url = string.Format("http://localhost:8001/api/c2s1003?data={0}",
                content.ToByteString().ToBase64());
            GameEntry.WebRequest.AddWebRequest(url, typeof(C2S1003));
        }
    }
}

