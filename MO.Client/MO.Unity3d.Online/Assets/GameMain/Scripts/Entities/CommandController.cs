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
		private float curX;
		private float curY;
		private float curZ;
		private float curRX;
		private float curRY;
		private float curRZ;

		private bool Compare(float x, float y)
		{
			if (Math.Abs(x - y) < 0.00001)
			{
				return true;
			}
			return false;
		}

		void FixedUpdate()
		{
			if (!Compare(transform.position.x, curX) ||
				!Compare(transform.position.y, curY) ||
				!Compare(transform.position.z, curZ) ||
				!Compare(transform.rotation.eulerAngles.x, curRX) ||
				!Compare(transform.rotation.eulerAngles.y, curRY) ||
				!Compare(transform.rotation.eulerAngles.z, curRZ))
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
