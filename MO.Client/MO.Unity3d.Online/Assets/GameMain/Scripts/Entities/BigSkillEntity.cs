using MO.Unity3d.Data;
using System;
using System.Collections;
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
			StartCoroutine(HideSkill(Entity.Id, 7));
		}

		private IEnumerator HideSkill(int entityId, int seconds)
		{
			yield return new WaitForSeconds(seconds);
			GameEntry.Entity.HideEntity(entityId);
		}

		protected internal override void OnShow(object userData)
		{
			base.OnShow(userData);
			transform.Rotate(new Vector3(_skillData.PlayerData.ServerRX, _skillData.PlayerData.ServerRY, _skillData.PlayerData.ServerRZ));
			transform.position = new Vector3(_skillData.PlayerData.ServerX, _skillData.PlayerData.ServerY, _skillData.PlayerData.ServerZ);
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
