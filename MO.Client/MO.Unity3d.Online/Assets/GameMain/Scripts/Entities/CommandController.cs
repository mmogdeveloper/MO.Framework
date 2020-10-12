using Google.Protobuf;
using MO.Algorithm.Enum;
using MO.Protocol;
using MO.Unity3d.Data;
using UnityEngine;

namespace MO.Unity3d.Entities
{
    public class CommandController : MonoBehaviour
    {
		private float curX;
		private float curY;
		private float curZ;
		private float curRX;
		private float curRY;
		private float curRZ;

		void FixedUpdate()
		{
			if (transform.position.x != curX ||
				transform.position.y != curY ||
				transform.position.z != curZ ||
				transform.rotation.eulerAngles.x != curRX ||
				transform.rotation.eulerAngles.y != curRY ||
				transform.rotation.eulerAngles.z != curRZ)
			{
				curX = transform.position.x;
				curY = transform.position.y;
				curZ = transform.position.z;
				curRX = transform.rotation.eulerAngles.x;
				curRY = transform.rotation.eulerAngles.y;
				curRZ = transform.rotation.eulerAngles.z;

				var transformInfo = new TransformInfo()
				{
						X = curX,
						Y = curY,
						Z = curZ,
						RX = curRX,
						RY = curRY,
						RZ = curRZ
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
				GameUser.Instance.SendPackage(content);
			}
		}
	}
}
