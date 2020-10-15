using Google.Protobuf;
using MO.Algorithm.OnlineDemo;
using MO.Protocol;
using MO.Unity3d.Data;
using System;
using UnityEngine;

namespace MO.Unity3d.Entities
{
    public class CommandController : MonoBehaviour
    {
		private Vector3 position;
		private Vector3 rotate;

		void FixedUpdate()
		{
			if (Vector3.Distance(position, GameUser.Instance.Position) > 0.01f ||
				Vector3.Distance(rotate, GameUser.Instance.Rotate) > 0.0004f)
			{
				position = GameUser.Instance.Position;
				rotate = GameUser.Instance.Rotate;

				var transformInfo = new TransformInfo()
				{
					Position = new MsgVector3()
					{
						X = position.x,
						Y = position.y,
						Z = position.z
					},
					Rotation = new MsgVector3()
					{
						X = rotate.x,
						Y = rotate.y,
						Z = rotate.z
					}
				};
				var command = new CommandInfo();
				command.CommandId = (int)CommandType.Transform;
				command.CommandContent = transformInfo.ToByteString();
				GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
			}

			if (GameUser.Instance.CurPlayer.SendCommands.Count != 0)
			{
				var content = new C2S100009();
				while (GameUser.Instance.CurPlayer.SendCommands.Count != 0)
				{
					var command = GameUser.Instance.CurPlayer.SendCommands.Dequeue();
					content.Commands.Add(command);
				}
				GlobalGame.SendPackage(content);
			}
		}
	}
}
