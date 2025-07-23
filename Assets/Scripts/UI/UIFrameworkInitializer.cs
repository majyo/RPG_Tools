using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI框架初始化器
    /// 建议将此脚本挂载到场景中的一个GameObject上，在游戏启动时自动初始化UI系统
    /// </summary>
    public class UIFrameworkInitializer : MonoBehaviour
    {
        [Header("自动初始化设置")]
        public bool autoInitializeOnStart = true;
        
        [Header("初始面板")]
        public UIPanelType initialPanel = UIPanelType.MainMenu;
        public bool openInitialPanel = true;

        private void Start()
        {
            if (autoInitializeOnStart)
            {
                InitializeUIFramework();
            }
        }

        /// <summary>
        /// 初始化UI框架
        /// </summary>
        public void InitializeUIFramework()
        {
            // 确保UIManager已创建
            var uiManager = UIManager.Instance;
            var resourceLoader = UIResourceLoader.Instance;
            
            Debug.Log("UI框架初始化完成");

            // 打开初始面板
            if (openInitialPanel && initialPanel != UIPanelType.None)
            {
                uiManager.OpenPanel(initialPanel);
            }
        }

        /// <summary>
        /// 手动初始化（如果没有启用自动初始化）
        /// </summary>
        [ContextMenu("手动初始化UI框架")]
        public void ManualInitialize()
        {
            InitializeUIFramework();
        }
    }
}
