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
        public double X { get; private set; }
        public double Y { get; private set; }
        public PlayerData(IUser user)
        {
            _user = user;
        }

        public void SetPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
