using MO.Protocol;
using MO.Unity3d.Data;
using UnityEngine;

namespace MO.Unity3d.Network
{
    public class UploadState : MonoBehaviour
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
				transform.rotation.eulerAngles.x != curRY ||
				transform.rotation.eulerAngles.x != curRZ)
			{
				curX = transform.position.x;
				curY = transform.position.y;
				curZ = transform.position.z;
				curRX = transform.rotation.eulerAngles.x;
				curRY = transform.rotation.eulerAngles.y;
				curRZ = transform.rotation.eulerAngles.z;

				GameUser.Instance.SendPackage(new C2S100003()
				{
					Vector = new MsgVector3()
					{
						X = curX,
						Y = curY,
						Z = curZ
					},
					Rotation = new MsgRotation()
					{
						X = curRX,
						Y = curRY,
						Z = curRZ
					}
				});
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
