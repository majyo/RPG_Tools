using System.Collections.Generic;

namespace RPGTools.UI
{
    /// <summary>
    /// UI面板类型枚举
    /// </summary>
    public enum UIPanelType
    {
        None = 0,
        MainMenu = 1,
        GameUI = 2,
        Inventory = 3,
        Settings = 4,
        Dialog = 5,
        Loading = 6
    }

    /// <summary>
    /// UI面板层级
    /// </summary>
    public enum UILayer
    {
        Background = 0,     // 背景层 (0-99)
        Normal = 100,       // 普通层 (100-199)
        Top = 200,          // 顶层 (200-299)
        System = 300        // 系统层 (300-399)
    }

    /// <summary>
    /// UI面板信息配置
    /// </summary>
    [System.Serializable]
    public class UIPanelInfo
    {
        public UIPanelType panelType;
        public string prefabPath;
        public UILayer layer;
        public bool isDestroyOnClose;
        
        public UIPanelInfo(UIPanelType type, string path, UILayer layer, bool destroyOnClose = false)
        {
            this.panelType = type;
            this.prefabPath = path;
            this.layer = layer;
            this.isDestroyOnClose = destroyOnClose;
        }
    }

    /// <summary>
    /// UI配置数据
    /// </summary>
    public static class UIConfig
    {
        /// <summary>
        /// UI面板配置字典
        /// </summary>
        public static readonly Dictionary<UIPanelType, UIPanelInfo> PanelInfoDict = new Dictionary<UIPanelType, UIPanelInfo>
        {
            { UIPanelType.MainMenu, new UIPanelInfo(UIPanelType.MainMenu, "UI/Panels/MainMenuPanel", UILayer.Normal) },
            { UIPanelType.GameUI, new UIPanelInfo(UIPanelType.GameUI, "UI/Panels/GameUIPanel", UILayer.Normal) },
            { UIPanelType.Inventory, new UIPanelInfo(UIPanelType.Inventory, "UI/Panels/InventoryPanel", UILayer.Top) },
            { UIPanelType.Settings, new UIPanelInfo(UIPanelType.Settings, "UI/Panels/SettingsPanel", UILayer.Top) },
            { UIPanelType.Dialog, new UIPanelInfo(UIPanelType.Dialog, "UI/Panels/DialogPanel", UILayer.System, true) },
            { UIPanelType.Loading, new UIPanelInfo(UIPanelType.Loading, "UI/Panels/LoadingPanel", UILayer.System) }
        };
    }
}
