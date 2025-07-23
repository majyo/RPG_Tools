# Unity UGUI UI框架使用指南

## 框架概述

这是一个基于Unity UGUI的完整UI管理框架，提供了以下核心功能：
- UI面板的统一管理（打开、关闭、缓存）
- UI资源的自动加载和卸载
- UI层级管理（背景层、普通层、顶层、系统层）
- 面板生命周期管理
- 数据传递机制

## 框架结构

### 核心组件
1. **UIManager** - UI管理器，负责面板的生命周期管理
2. **UIPanel** - UI面板基类，所有面板都需要继承此类
3. **UIResourceLoader** - UI资源加载器，负责预制体的加载
4. **UIDefine** - UI定义文件，包含面板类型和配置信息
5. **UIFrameworkInitializer** - 框架初始化器

### 面板示例
- **MainMenuPanel** - 主菜单面板
- **GameUIPanel** - 游戏UI面板
- **SettingsPanel** - 设置面板
- **LoadingPanel** - 加载面板
- **DialogPanel** - 对话框面板

## 快速开始

### 1. 初始化框架

在场景中创建一个空GameObject，挂载`UIFrameworkInitializer`脚本：

```csharp
// 框架会自动初始化，并可以设置初始打开的面板
public UIPanelType initialPanel = UIPanelType.MainMenu;
public bool openInitialPanel = true;
```

### 2. 创建自定义面板

```csharp
using UIFramework;

public class YourCustomPanel : UIPanel
{
    protected override void OnInit()
    {
        base.OnInit();
        // 初始化UI组件，绑定事件等
    }

    protected override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        // 面板打开时的逻辑
    }

    protected override void OnClose()
    {
        base.OnClose();
        // 面板关闭时的逻辑
    }
}
```

### 3. 配置面板信息

在`UIDefine.cs`中添加你的面板配置：

```csharp
public enum UIPanelType
{
    // 添加你的面板类型
    YourCustomPanel = 7
}

public static readonly Dictionary<UIPanelType, UIPanelInfo> PanelInfoDict = new Dictionary<UIPanelType, UIPanelInfo>
{
    // 添加面板配置
    { UIPanelType.YourCustomPanel, new UIPanelInfo(UIPanelType.YourCustomPanel, "UI/Panels/YourCustomPanel", UILayer.Normal) }
};
```

## 使用方法

### 打开面板
```csharp
// 简单打开
UIManager.Instance.OpenPanel(UIPanelType.MainMenu);

// 带数据打开
var gameData = new GameUIData(100, 100, 0);
UIManager.Instance.OpenPanel(UIPanelType.GameUI, gameData);

// 带回调打开
UIManager.Instance.OpenPanel(UIPanelType.Settings, null, (panel) => {
    Debug.Log("设置面板已打开");
});
```

### 关闭面板
```csharp
// 关闭指定面板
UIManager.Instance.ClosePanel(UIPanelType.Settings);

// 关闭所有面板
UIManager.Instance.CloseAllPanels();
```

### 获取已打开的面板
```csharp
// 检查面板是否已打开
bool isOpened = UIManager.Instance.IsPanelOpened(UIPanelType.GameUI);

// 获取已打开的面板实例
UIPanel panel = UIManager.Instance.GetOpenedPanel(UIPanelType.GameUI);
if (panel is GameUIPanel gameUI)
{
    gameUI.SetHealth(80, 100);
}
```

### 显示对话框
```csharp
// 简单消息框
DialogPanel.ShowMessage("提示", "游戏保存成功！");

// 确认对话框
DialogPanel.ShowConfirm("确认", "确定要退出游戏吗？", 
    () => Application.Quit(),  // 确认回调
    () => Debug.Log("取消退出") // 取消回调
);
```

### 显示加载界面
```csharp
// 打开加载面板
var loadingData = new LoadingData("正在加载游戏资源...");
UIManager.Instance.OpenPanel(UIPanelType.Loading, loadingData);

// 更新加载进度
var loadingPanel = UIManager.Instance.GetOpenedPanel(UIPanelType.Loading) as LoadingPanel;
loadingPanel?.UpdateProgress(0.5f, "加载中... 50%");
```

## 资源组织

建议在`Assets/Resources/UI/Panels/`目录下创建UI预制体：

```
Assets/
  Resources/
    UI/
      Panels/
        MainMenuPanel.prefab
        GameUIPanel.prefab
        SettingsPanel.prefab
        DialogPanel.prefab
        LoadingPanel.prefab
```

## 高级功能

### 面板层级管理
框架支持4个层级：
- **Background** (0-99) - 背景层
- **Normal** (100-199) - 普通层
- **Top** (200-299) - 顶层
- **System** (300-399) - 系统层

### 面板缓存机制
- 默认情况下，面板关闭后会被缓存，再次打开时直接使用缓存
- 可以设置`isDestroyOnClose = true`让面板关闭时销毁

### 数据传递
通过`Open`方法的`data`参数可以向面板传递任意数据：

```csharp
public class CustomData
{
    public int playerId;
    public string playerName;
}

var data = new CustomData { playerId = 123, playerName = "Player1" };
UIManager.Instance.OpenPanel(UIPanelType.YourPanel, data);
```

## 注意事项

1. 所有UI面板预制体都必须挂载继承自`UIPanel`的脚本
2. 面板预制体需要放在`Resources`文件夹下，路径要与配置一致
3. 确保Canvas组件正确配置（框架会自动处理大部分设置）
4. 建议在场景开始时就初始化UI框架

## 扩展建议

- 可以添加面板切换动画
- 可以集成对象池来优化性能
- 可以添加音效管理
- 可以添加本地化支持
