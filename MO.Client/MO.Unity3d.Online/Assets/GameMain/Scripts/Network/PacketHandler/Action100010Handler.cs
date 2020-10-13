using GameFramework.Network;
using MO.Algorithm.OnlineDemo;
using MO.Protocol;
using MO.Unity3d.Data;
using UnityGameFramework.Runtime;

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
            if (GameUser.Instance.FrameCount != 0)
            {
                var nextFrameCount = GameUser.Instance.FrameCount + 1;
                if (nextFrameCount != rep.FrameCount)
                {
                    Log.Info("丢帧");
                }
            }
            GameUser.Instance.FrameCount = rep.FrameCount;
            PlayerData player;
            foreach (var command in rep.Commands)
            {
                if (GameUser.Instance.Players.TryGetValue(command.UserId, out player))
                {
                    if (command.CommandId == (int)CommandType.Transform)
                    {
                        var transform = TransformInfo.Parser.ParseFrom(command.CommandContent);
                        player.ServerX = transform.X;
                        player.ServerY = transform.Y;
                        player.ServerZ = transform.Z;
                        player.ServerRX = transform.RX;
                        player.ServerRY = transform.RY;
                        player.ServerRZ = transform.RZ;
                    }
                    else
                    {
                        if (command.UserId == GameUser.Instance.UserId)
                            continue;

                        switch (command.CommandId)
                        {
                            case (int)CommandType.BigSkill:
                                player.ShowBigSkill();
                                break;
                            case (int)CommandType.Jump:
                                player.Jump();
                                break;
                            case (int)CommandType.SkillC:
                                player.ShowSkillC();
                                break;
                            case (int)CommandType.SkillX:
                                player.ShowSkillX();
                                break;
                            case (int)CommandType.SkillZ:
                                player.ShowSkillZ();
                                break;
                        }
                    }
                }
            }

            var bloodInfoList = BloodInfoList.Parser.ParseFrom(rep.CommandResult);
            foreach (var bloodInfo in bloodInfoList.Bloods)
            {
                if (GameUser.Instance.Players.TryGetValue(bloodInfo.UserId, out player))
                {
                    player.CurBlood = bloodInfo.BloodValue;
                }
            }
        }
    }
}
