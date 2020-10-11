using GameFramework.Network;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.Entities;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;

namespace MO.Unity3d.Network.PacketHandler
{
    public class Action100010Handler : IPacketHandler
    {
        public int Id
        {
            get { return 100010; }
        }

        public void Handle(object sender, Packet packet)
        {
            S2C100010 rep = S2C100010.Parser.ParseFrom(((MOPacket)packet).Packet.Content);
            if (rep.UserId == GameUser.Instance.UserId)
                return;

            PlayerData player;
            if (GameUser.Instance.Players.TryGetValue(rep.UserId, out player))
            {
                foreach (var command in rep.Commands)
                {
                    switch (command.CommandId)
                    {
                        case (int)CommandEnum.BigSkill:
                            player.ShowBigSkill();
                            break;
                        case (int)CommandEnum.Jump:
                            player.Jump();
                            break;
                        case (int)CommandEnum.SkillC:
                            player.ShowSkillC();
                            break;
                        case (int)CommandEnum.SkillX:
                            player.ShowSkillX();
                            break;
                        case (int)CommandEnum.SkillZ:
                            player.ShowSkillZ();
                            break;
                    }
                }
            }
        }
    }
}
