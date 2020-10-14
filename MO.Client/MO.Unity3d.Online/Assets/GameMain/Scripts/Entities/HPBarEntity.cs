using MO.Unity3d.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.Entities
{
	public class HPBarEntity : EntityLogic
	{
		private Transform _sliderTransform;
		private Slider _slider;
		private PlayerData _playerData;
		private Entity _parent;

		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			_playerData = (PlayerData)userData;
			_parent = GameEntry.Entity.GetEntity(_playerData.EntityId);
			_sliderTransform = transform.Find("Slider");
			_slider = GetComponentInChildren<Slider>();
			_slider.maxValue = _playerData.MaxBlood;
			_slider.minValue = 0;
			_slider.value = _playerData.CurBlood;
			GameEntry.Entity.AttachEntity(Entity, _parent);
		}

		protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
		{
			base.OnUpdate(elapseSeconds, realElapseSeconds);
			var position = Camera.main.WorldToScreenPoint(_parent.transform.position);
			_sliderTransform.position = position;
			_slider.value = _playerData.CurBlood;
		}
	}
}
