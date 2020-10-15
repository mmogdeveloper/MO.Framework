using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGameFramework.Runtime;

namespace MO.Unity3d.UIExtension
{
    public static class SceneExtension
    {
        public static void LoadLoginScene(this SceneComponent component)
        {
            component.LoadScene("Assets/GameMain/Scenes/Login.unity");
        }

        public static void UnLoadLoginScene(this SceneComponent component)
        {
            component.UnloadScene("Assets/GameMain/Scenes/Login.unity");
        }

        public static void LoadGameScene(this SceneComponent component)
        {
            component.LoadScene("Assets/GameMain/Scenes/Game.unity");
        }

        public static void UnLoadGameScene(this SceneComponent component)
        {
            component.UnloadScene("Assets/GameMain/Scenes/Game.unity");
        }
    }
}
