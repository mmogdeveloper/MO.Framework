using MO.Algorithm.OnlineDemo;
using MO.Protocol;
using MO.Unity3d.Data;
using MO.Unity3d.UIExtension;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace MO.Unity3d.Skills
{
    public class SkillController : MonoBehaviour
    {
        public GameObject btn_Jump;
        public GameObject btn_BigSkill;
        public GameObject btn_SkillC;
        public GameObject btn_SkillX;
        public GameObject btn_SkillZ;

        private IEnumerator EnableButton(GameObject btnObject, float cd)
        {
            var btn = btnObject.GetComponent<Button>();
            var txt = btnObject.GetComponentInChildren<Text>();
            var name = txt.text;
            btn.image.sprite = btn.spriteState.disabledSprite;
            btn.enabled = false;
            var step = 0.1f;
            for (float i = cd; i > 0; i -= step)
            {
                txt.text = Math.Round(i, 1).ToString("0.0");
                yield return new WaitForSeconds(step);
            }
            txt.text = name;
            btn.image.sprite = btn.spriteState.highlightedSprite;
            btn.enabled = true;
        }

        public void OnJump()
        {
            GameUser.Instance.CurPlayer.Jump();
            var command = new CommandInfo();
            command.CommandId = (int)CommandType.Jump;
            GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);

            StartCoroutine(EnableButton(btn_Jump, DemoValue.JumpCD));
        }

        public void OnSkillC()
        {
            if (GameUser.Instance.CurPlayer.ShowSkillC())
            {
                var command = new CommandInfo();
                command.CommandId = (int)CommandType.SkillC;
                GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);

                StartCoroutine(EnableButton(btn_SkillC, DemoValue.SkillCAttackCD));
            }
        }

        public void OnSkillX()
        {
            if (GameUser.Instance.CurPlayer.ShowSkillX())
            {
                var command = new CommandInfo();
                command.CommandId = (int)CommandType.SkillX;
                GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);

                StartCoroutine(EnableButton(btn_SkillX, DemoValue.SkillXAttackCD));
            }
        }

        public void OnSkillZ()
        {
            if (GameUser.Instance.CurPlayer.ShowSkillZ())
            {
                var command = new CommandInfo();
                command.CommandId = (int)CommandType.SkillZ;
                GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);

                StartCoroutine(EnableButton(btn_SkillZ, DemoValue.SkillZAttackCD));
            }
        }

        public void OnSkillBig()
        {
            if (GameUser.Instance.CurPlayer.ShowBigSkill())
            {
                var command = new CommandInfo();
                command.CommandId = (int)CommandType.BigSkill;
                GameUser.Instance.CurPlayer.SendCommands.Enqueue(command);

                StartCoroutine(EnableButton(btn_BigSkill, DemoValue.BigSkillAttackCD));
            }
        }
    }
}
