using MO.Unity3d.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
    public class PlayerChatEntity : EntityLogic
    {
        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            var scrollerObj = GameObject.Find("scroll_view");

            var nameCom = this.GetComponentInChildren<Text>();
            nameCom.text = ((MsgUserData)userData).UserName;

            var imgCom = this.GetComponentInChildren<Image>();
            var msgCom = imgCom.GetComponentInChildren<Text>();
            msgCom.text = ((MsgUserData)userData).Msg;

            //var 
            this.transform.SetParent(scrollerObj.transform);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}
