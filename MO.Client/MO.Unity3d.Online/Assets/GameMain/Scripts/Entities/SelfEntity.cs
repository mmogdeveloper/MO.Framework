using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
    public class SelfEntity : EntityLogic
    {
		public string HorizontalAxis = "Horizontal";
		public string VerticalAxis = "Vertical";
		//public string jumpButton = "Jump";

		private float _inputHorizontal;
		private float _inputVertical;
		private Camera _camera;
		private Vector3 _offset;


		private Vector3 _position = Vector3.zero;
		private Vector3 _eulerAngles = Vector3.zero;
		private Vector3 _scale = Vector3.zero;

		public Vector3 DestPosition = Vector3.zero;
		public Vector3 DestDirection = Vector3.zero;

		private float _speed = 0f;

		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			GetComponent<Renderer>().material.color = Color.blue;
			transform.position = new Vector3(
				GameUser.Instance.CurPlayer.X,
				GameUser.Instance.CurPlayer.Y,
				GameUser.Instance.CurPlayer.Z);

			Vector3 eulerAngles = new Vector3(
				GameUser.Instance.CurPlayer.RX,
				GameUser.Instance.CurPlayer.RY,
				GameUser.Instance.CurPlayer.RZ);
			transform.Rotate(eulerAngles);

			_camera = Camera.main;
			_offset = _camera.transform.position;
			_camera.transform.position = transform.position + _offset;
		}

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			_inputHorizontal = SimpleInput.GetAxis(HorizontalAxis);
			_inputVertical = SimpleInput.GetAxis(VerticalAxis);
			if (_inputHorizontal == 0 && _inputVertical == 0)
				return;

			Vector3 dir = new Vector3(_inputHorizontal, 0, _inputVertical);
			Quaternion quaternion = Quaternion.LookRotation(dir);

			transform.rotation = quaternion;

			transform.position += transform.forward * Time.deltaTime * 2.0f;

			_camera.transform.position = transform.position + _offset;

			base.OnUpdate(elapseSeconds, realElapseSeconds);
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
