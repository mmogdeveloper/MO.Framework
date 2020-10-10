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

    private IEnumerator HideSkill(int entityId, int seconds)
    {
        yield return new WaitForSeconds(seconds);
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
        StartCoroutine(HideSkill(entityId, 3));
    }

    private void ShowBigSkill(int entityId)
    {
        if (GameEntry.Entity.HasEntity(entityId))
            return;

        var skillData = new SkillData();
        skillData.PlayerData = GameUser.Instance.CurPlayer;
        GameEntry.Entity.ShowEntity<BigSkillEntity>(entityId,
            "Assets/GameMain/Entities/BigSkill.prefab", "DefaultEntityGroup", skillData);
        StartCoroutine(HideSkill(entityId, 7));
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
        ShowBigSkill(GameUser.Instance.CurPlayer.SkillBigEntityId);
    }
}
