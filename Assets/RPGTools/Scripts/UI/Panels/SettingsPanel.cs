using UnityEngine;
using UnityEngine.UI;

namespace RPGTools.UI.Panels
{
    /// <summary>
    /// 设置面板示例
    /// </summary>
    public class SettingsPanel : UIPanel
    {
        [Header("UI组件")]
        public Slider volumeSlider;
        public Toggle fullscreenToggle;
        public Button closeButton;
        public Button resetButton;

        protected override void OnInit()
        {
            base.OnInit();
            
            // 绑定UI事件
            if (volumeSlider != null)
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
                
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
                
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
                
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);
        }

        protected override void OnOpen(object data = null)
        {
            base.OnOpen(data);
            LoadSettings();
        }

        protected override void OnClose()
        {
            base.OnClose();
            SaveSettings();
        }

        private void LoadSettings()
        {
            // 加载设置数据
            if (volumeSlider != null)
                volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1.0f);
                
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;
        }

        private void SaveSettings()
        {
            // 保存设置数据
            if (volumeSlider != null)
                PlayerPrefs.SetFloat("Volume", volumeSlider.value);
                
            PlayerPrefs.Save();
        }

        private void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
        }

        private void OnFullscreenToggled(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        private void OnCloseClicked()
        {
            UIManager.Instance.ClosePanel(UIPanelType.Settings);
        }

        private void OnResetClicked()
        {
            // 重置设置到默认值
            if (volumeSlider != null)
                volumeSlider.value = 1.0f;
                
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = false;
        }
    }
}
