using MO.GrainInterfaces.User;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        private IUser _user;
        private MOPoint _point;
        public PlayerData(IUser user)
        {
            _user = user;
            _point = new MOPoint();
        }

        public void SetPoint(MOPoint point)
        {
            _point = point.Clone();
        }

        public MOPoint GetPoint()
        {
            return _point.Clone();
        }
    }
}
