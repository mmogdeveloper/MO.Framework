using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.UIExtension
{
    public static class FormExtension
    {
        public static int OpenLoginForm(this UIComponent component)
        {
            return component.OpenUIForm("Assets/GameMain/UI/UIForms/LoginForm.prefab", "DefaultUIGroup");
        }

        public static int OpenGameForm(this UIComponent component)
        {
            return component.OpenUIForm("Assets/GameMain/UI/UIForms/GameForm.prefab", "DefaultUIGroup");
        }
    }
}
