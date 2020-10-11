using MO.Unity3d.Data;
using System.Collections;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
    public class SkillEntity : EntityLogic
	{
		private SkillData _skillData;
		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			_skillData = (SkillData)userData;
			StartCoroutine(HideSkill(Entity.Id, 3));
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
			transform.position = transform.forward * _skillData.Distance + new Vector3(_skillData.PlayerData.ServerX, _skillData.PlayerData.ServerY, _skillData.PlayerData.ServerZ);
		}
    }
}
