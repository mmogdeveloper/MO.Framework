using MO.Protocol;
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
        GameUser.Instance.CurPlayer.Jump();
        var command = new CommandInfo();
        command.CommandId = (int)CommandEnum.Jump;
        GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
    }

    public void OnSkillC()
    {
        GameUser.Instance.CurPlayer.ShowSkillC();
        var command = new CommandInfo();
        command.CommandId = (int)CommandEnum.SkillC;
        GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
    }

    public void OnSkillX()
    {
        GameUser.Instance.CurPlayer.ShowSkillX();
        var command = new CommandInfo();
        command.CommandId = (int)CommandEnum.SkillX;
        GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
    }

    public void OnSkillZ()
    {
        GameUser.Instance.CurPlayer.ShowSkillZ();
        var command = new CommandInfo();
        command.CommandId = (int)CommandEnum.SkillZ;
        GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
    }

    public void OnSkillBig()
    {
        GameUser.Instance.CurPlayer.ShowBigSkill();
        var command = new CommandInfo();
        command.CommandId = (int)CommandEnum.BigSkill;
        GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
    }
}
