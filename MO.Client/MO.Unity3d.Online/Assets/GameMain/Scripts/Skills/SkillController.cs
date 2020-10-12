using MO.Algorithm.Enum;
using MO.Protocol;
using MO.Unity3d.Data;
using UnityEngine;

namespace MO.Unity3d.Skills
{
    public class SkillController : MonoBehaviour
    {
        public void OnJump()
        {
            GameUser.Instance.CurPlayer.Jump();
            var command = new CommandInfo();
            command.CommandId = (int)CommandType.Jump;
            GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
        }

        public void OnSkillC()
        {
            GameUser.Instance.CurPlayer.ShowSkillC();
            var command = new CommandInfo();
            command.CommandId = (int)CommandType.SkillC;
            GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
        }

        public void OnSkillX()
        {
            GameUser.Instance.CurPlayer.ShowSkillX();
            var command = new CommandInfo();
            command.CommandId = (int)CommandType.SkillX;
            GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
        }

        public void OnSkillZ()
        {
            GameUser.Instance.CurPlayer.ShowSkillZ();
            var command = new CommandInfo();
            command.CommandId = (int)CommandType.SkillZ;
            GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
        }

        public void OnSkillBig()
        {
            GameUser.Instance.CurPlayer.ShowBigSkill();
            var command = new CommandInfo();
            command.CommandId = (int)CommandType.BigSkill;
            GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);
        }
    }
}
