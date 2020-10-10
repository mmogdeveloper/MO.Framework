using MO.Unity3d;
using MO.Unity3d.Data;
using MO.Unity3d.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillControl : MonoBehaviour
{
    public void OnJump()
    {
        GameUser.Instance.CurPlayer.JumpState = 1;
    }

    private IEnumerator HideSkill(int entityId)
    {
        yield return new WaitForSeconds(3);
        GameEntry.Entity.HideEntity(entityId);
    }

    private void ShowSkill(int entityId, float distance)
    {
        if (GameEntry.Entity.HasEntity(entityId))
            return;

        var skillData = new SkillData();
        skillData.PlayerData = GameUser.Instance.CurPlayer;
        skillData.Distance = distance;
        GameEntry.Entity.ShowEntity<SkillEntity>(entityId,
            "Assets/GameMain/Entities/Skill.prefab", "DefaultEntityGroup", skillData);
        StartCoroutine(HideSkill(entityId));
    }

    public void OnSkillC()
    {
        ShowSkill(GameUser.Instance.CurPlayer.SkillCEntityId, 9);
    }

    public void OnSkillX()
    {
        ShowSkill(GameUser.Instance.CurPlayer.SkillXEntityId, 6);
    }

    public void OnSkillZ()
    {
        ShowSkill(GameUser.Instance.CurPlayer.SkillZEntityId, 3);
    }

    public void OnSkillBig()
    {

    }
}
