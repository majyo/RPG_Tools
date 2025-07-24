using UnityEngine;
using UnityEngine.UI;

namespace RPGTools.UI.Panels
{
    /// <summary>
    /// 示例主菜单面板
    /// </summary>
    public class MainMenuPanel : UIPanel
    {
        [Header("UI组件")]
        public Button startGameButton;
        public Button settingsButton;
        public Button exitButton;

        protected override void OnInit()
        {
            base.OnInit();
            
            // 绑定按钮事件
            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGameClicked);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
                
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);
        }

        protected override void OnOpen(object data = null)
        {
            base.OnOpen(data);
            Debug.Log("主菜单面板已打开");
        }

        protected override void OnClose()
        {
            base.OnClose();
            Debug.Log("主菜单面板已关闭");
        }

        private void OnStartGameClicked()
        {
            // 关闭主菜单，打开游戏UI
            UIManager.Instance.ClosePanel(UIPanelType.MainMenu);
            UIManager.Instance.OpenPanel(UIPanelType.GameUI);
        }

        private void OnSettingsClicked()
        {
            // 打开设置面板
            UIManager.Instance.OpenPanel(UIPanelType.Settings);
        }

        private void OnExitClicked()
        {
            // 退出游戏
            Application.Quit();
        }
    }
}
