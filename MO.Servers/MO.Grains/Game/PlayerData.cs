using MO.GrainInterfaces.User;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        private IUser _user;
        public Int64 X { get; private set; }
        public Int64 Y { get; private set; }
        public PlayerData(IUser user)
        {
            _user = user;
        }

        public void SetPoint(Int64 x, Int64 y)
        {
            X = x;
            Y = y;
        }
    }
}
