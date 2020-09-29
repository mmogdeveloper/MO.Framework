//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;

namespace Tutorial
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        private void Start()
        {
            // 初始化内置组件
            InitBuiltinComponents();

            // 初始化自定义组件
            InitCustomComponents();

            // 初始化自定义调试器
            InitCustomDebuggers();
        }
    }
}
