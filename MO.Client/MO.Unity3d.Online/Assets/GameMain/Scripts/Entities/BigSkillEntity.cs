using MO.Unity3d.Data;
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
    public class BigSkillEntity: EntityLogic
    {
		private SkillData _skillData;
		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			_skillData = (SkillData)userData;
		}

		protected internal override void OnShow(object userData)
		{
			base.OnShow(userData);
			transform.Rotate(new Vector3(_skillData.PlayerData.RX, _skillData.PlayerData.RY, _skillData.PlayerData.RZ));
			transform.position = new Vector3(_skillData.PlayerData.X, _skillData.PlayerData.Y, _skillData.PlayerData.Z);
			transform.localScale = new Vector3();
		}

		protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(elapseSeconds, realElapseSeconds);

			if (transform.localScale.x >= 2.0f)
				return;

			var scale = Time.deltaTime;
			transform.localScale = transform.localScale + new Vector3(scale, scale, scale);
		}
    }
}
