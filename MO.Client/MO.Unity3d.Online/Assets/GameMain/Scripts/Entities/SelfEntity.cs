using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;
using System.Collections;

namespace MO.Unity3d.Entities
{
    public class SelfEntity : EntityLogic
    {
		private Vector3 _offset;
		private float _positionSpeed = 2.0f;
		private float _rotateSpeed = 8.0f;
		private PlayerData _playerData;

		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			//UIJoystickControl.Instance.joystickDragDelegate = OnJoystickDrag;
			_playerData = (PlayerData)userData;
			GetComponent<Renderer>().material.color = Color.blue;
			transform.position = new Vector3(_playerData.X, _playerData.Y, _playerData.Z);
			Vector3 eulerAngles = new Vector3(
				_playerData.RX,
				_playerData.RY,
				_playerData.RZ);
			transform.Rotate(eulerAngles);

			_offset = Camera.main.transform.position;
			var position = transform.position + _offset;
			position.y = 10;
			Camera.main.transform.position = position;
		}

		protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			if (GameUser.Instance.IsJump)
			{
				float magnitude = transform.rotation.eulerAngles.magnitude;
				float x = (float)Math.Cos(Math.PI * (magnitude / 180)) * 5;
				float z = (float)Math.Sin(Math.PI * (magnitude / 180)) * 5;
				if ((magnitude > 90 && magnitude < 180) ||
					(magnitude > 270 && magnitude < 360))
				{
					x = -x;
					z = -z;
				}

				transform.position += new Vector3(x, 0, z);
				Log.Info("{0}-{1}-{2}", transform.position.x, transform.position.y, transform.position.z);
				GameUser.Instance.IsJump = false;
			}

			var eulerAngles = JoystickControl.GetDestination();
			if (eulerAngles != new Vector3())
			{
				Vector3 destDirection = new Vector3(eulerAngles.x, 0, eulerAngles.y);
				Quaternion quaternion = Quaternion.LookRotation(destDirection);
				transform.rotation = quaternion;
				transform.position += transform.forward * Time.deltaTime * _positionSpeed;
			}
			var position = transform.position + _offset;
			position.y = 10;
			Camera.main.transform.position = position;
			//FixedState();
			base.OnUpdate(elapseSeconds, realElapseSeconds);
		}

		private void FixedState()
		{
			//500ms误差修正玩家位置
			var destDirection = new Vector3(_playerData.RX, _playerData.RY, _playerData.RZ);
			if (Vector3.Distance(transform.eulerAngles, destDirection) > _rotateSpeed / 2)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(destDirection), _rotateSpeed * Time.deltaTime);
			}

			float distance = 0.0f;
			float deltaSpeed = (_positionSpeed * Time.deltaTime);

			var destPosition = new Vector3(_playerData.X, _playerData.Y, _playerData.Z);
			distance = Vector3.Distance(destPosition, transform.position);

			if (distance > _positionSpeed / 2)
			{
				Vector3 pos = transform.position;

				Vector3 movement = destPosition - pos;
				movement.y = 0f;
				movement.Normalize();

				movement *= deltaSpeed;

				if (distance > deltaSpeed || movement.magnitude > deltaSpeed)
					pos += movement;
				else
					pos = destPosition;

				transform.position = pos;
			}
		}

		void FixedUpdate()
		{
			GameUser.Instance.Channel.Send(new C2S100003()
			{
				Vector = new MsgVector3()
				{
					X = transform.position.x,
					Y = transform.position.y,
					Z = transform.position.z
				},
				Rotation = new MsgRotation()
				{
					X = transform.rotation.eulerAngles.x,
					Y = transform.rotation.eulerAngles.y,
					Z = transform.rotation.eulerAngles.z
				}
			}.BuildPacket());
		}
	}
}
