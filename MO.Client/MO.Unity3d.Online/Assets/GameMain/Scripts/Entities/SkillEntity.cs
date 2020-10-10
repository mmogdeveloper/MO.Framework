using MO.Unity3d.Data;
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
		}

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
			transform.Rotate(new Vector3(_skillData.PlayerData.RX, _skillData.PlayerData.RY, _skillData.PlayerData.RZ));
			transform.position = transform.forward * _skillData.Distance + new Vector3(_skillData.PlayerData.X, _skillData.PlayerData.Y, _skillData.PlayerData.Z);
		}
    }
}
