using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MO.Unity3d.Data
{
    public class MsgUserData
    {
        public MsgUserData(string userName, string msg)
        {
            UserName = userName;
            Msg = msg;
        }
        public string UserName { get; }
        public string Msg { get; }
    }
}
