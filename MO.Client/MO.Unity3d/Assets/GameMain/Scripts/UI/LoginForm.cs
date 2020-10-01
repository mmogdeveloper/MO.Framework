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
        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
        }

        public void OnLogin()
        {
            var input = GetComponentInChildren<InputField>();
            GameUser.Instance.CurPlayer.UserName = input.text;
            C2S_1003 content = new C2S_1003();
            content.DeviceId = input.text;
            content.MobileType = 1;
            var url = string.Format("http://localhost:8001/api/c2s1003?data={0}",
                content.ToByteString().ToBase64());
            GameEntry.WebRequest.AddWebRequest(url, this);
        }
    }
}

