using MO.Unity3d.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
	public class PlayerEntity : EntityLogic
	{
		private PlayerData _playerData;

		private float _positionSpeed = 2.0f;
		private float _rotateSpeed = 8.0f;

		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			_playerData = (PlayerData)userData;

			GetComponent<Renderer>().material.color = Color.green;

			transform.position = new Vector3(_playerData.X, _playerData.Y, _playerData.Z);

			Vector3 eulerAngles = new Vector3(
				_playerData.RX,
				_playerData.RY,
				_playerData.RZ);
			transform.Rotate(eulerAngles);
		}

		protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			var destDirection = new Vector3(_playerData.RX, _playerData.RY, _playerData.RZ);
			if (Vector3.Distance(transform.eulerAngles, destDirection) > 0.0004f)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(destDirection), _rotateSpeed * Time.deltaTime);
			}

			float distance = 0.0f;
			float deltaSpeed = (_positionSpeed * Time.deltaTime);

			var destPosition = new Vector3(_playerData.X, _playerData.Y, _playerData.Z);
			distance = Vector3.Distance(destPosition, transform.position);

			if (distance > 0.01f)
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

			base.OnUpdate(elapseSeconds, realElapseSeconds);
		}
    }
}