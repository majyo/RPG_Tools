using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGTools.UI
{
    /// <summary>
    /// UI管理器 - 负责管理所有UI面板的开启、关闭和生命周期
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                
                var go = new GameObject("UIManager");
                _instance = go.AddComponent<UIManager>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        [Header("UI画布配置")]
        public Canvas uiCanvas;
        public Transform backgroundLayer;
        public Transform normalLayer;
        public Transform topLayer;
        public Transform systemLayer;

        /// <summary>
        /// 已打开的面板字典
        /// </summary>
        private readonly Dictionary<UIPanelType, UIPanel> _openedPanels = new();

        /// <summary>
        /// 面板实例缓存
        /// </summary>
        private readonly Dictionary<UIPanelType, UIPanel> _panelCache = new();

        /// <summary>
        /// 当前排序顺序
        /// </summary>
        private readonly Dictionary<UILayer, int> _currentSortingOrder = new();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeLayers();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初始化UI层级
        /// </summary>
        private void InitializeLayers()
        {
            // 如果没有指定Canvas，创建一个
            if (uiCanvas == null)
            {
                var canvasGO = new GameObject("UICanvas");
                canvasGO.transform.SetParent(transform);
                uiCanvas = canvasGO.AddComponent<Canvas>();
                uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            // 创建层级节点
            CreateLayerIfNotExists(ref backgroundLayer, "BackgroundLayer");
            CreateLayerIfNotExists(ref normalLayer, "NormalLayer");
            CreateLayerIfNotExists(ref topLayer, "TopLayer");
            CreateLayerIfNotExists(ref systemLayer, "SystemLayer");

            // 初始化排序顺序
            _currentSortingOrder[UILayer.Background] = (int)UILayer.Background;
            _currentSortingOrder[UILayer.Normal] = (int)UILayer.Normal;
            _currentSortingOrder[UILayer.Top] = (int)UILayer.Top;
            _currentSortingOrder[UILayer.System] = (int)UILayer.System;
        }

        /// <summary>
        /// 创建层级节点
        /// </summary>
        private void CreateLayerIfNotExists(ref Transform layer, string layerName)
        {
            if (layer != null)
            {
                return;
            }
            
            var layerGO = new GameObject(layerName);
            layerGO.transform.SetParent(uiCanvas.transform, false);
            layer = layerGO.transform;

            // 添加RectTransform并设置为全屏
            var rect = layerGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 打开UI面板
        /// </summary>
        /// <param name="panelType">面板类型</param>
        /// <param name="data">传递给面板的数据</param>
        /// <param name="onComplete">打开完成回调</param>
        public void OpenPanel(UIPanelType panelType, object data = null, Action<UIPanel> onComplete = null)
        {
            if (panelType == UIPanelType.None)
            {
                Debug.LogWarning("Cannot open panel of type None");
                return;
            }

            // 如果面板已经打开，直接返回
            if (_openedPanels.TryGetValue(panelType, out var openedPanel))
            {
                Debug.LogWarning($"Panel {panelType} is already opened");
                onComplete?.Invoke(openedPanel);
                return;
            }

            // 获取面板配置信息
            if (!UIConfig.PanelInfoDict.TryGetValue(panelType, out var panelInfo))
            {
                Debug.LogError($"Panel info not found for type: {panelType}");
                return;
            }

            // 检查缓存中是否有面板实例
            if (_panelCache.TryGetValue(panelType, out var cachedPanel))
            {
                ShowPanel(cachedPanel, data, onComplete);
                return;
            }

            // 异步加载面板
            UIResourceLoader.Instance.LoadUIPrefabAsync(panelInfo.prefabPath, prefabInstance =>
            {
                if (prefabInstance == null)
                {
                    Debug.LogError($"Failed to load panel prefab: {panelInfo.prefabPath}");
                    return;
                }

                var panel = prefabInstance.GetComponent<UIPanel>();
                if (panel == null)
                {
                    Debug.LogError($"Prefab {panelInfo.prefabPath} does not have UIPanel component");
                    Destroy(prefabInstance);
                    return;
                }

                // 设置面板属性
                panel.panelType = panelType;
                panel.layer = panelInfo.layer;

                // 设置父节点
                Transform parentLayer = GetLayerTransform(panelInfo.layer);
                prefabInstance.transform.SetParent(parentLayer, false);

                // 设置层级顺序
                int sortingOrder = GetNextSortingOrder(panelInfo.layer);
                panel.SetSortingOrder(sortingOrder);

                // 缓存面板（如果不是销毁类型）
                if (!panelInfo.isDestroyOnClose)
                {
                    _panelCache[panelType] = panel;
                }

                // 注册事件
                panel.OnPanelClosed += OnPanelClosed;

                ShowPanel(panel, data, onComplete);
            });
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        private void ShowPanel(UIPanel panel, object data, Action<UIPanel> onComplete)
        {
            _openedPanels[panel.panelType] = panel;
            panel.Open(data);
            onComplete?.Invoke(panel);
        }

        /// <summary>
        /// 关闭UI面板
        /// </summary>
        /// <param name="panelType">面板类型</param>
        public void ClosePanel(UIPanelType panelType)
        {
            if (_openedPanels.TryGetValue(panelType, out var panel))
            {
                panel.Close();
            }
            else
            {
                Debug.LogWarning($"Panel {panelType} is not opened");
            }
        }

        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public void CloseAllPanels()
        {
            List<UIPanelType> panelsToClose = new List<UIPanelType>(_openedPanels.Keys);
            foreach (UIPanelType panelType in panelsToClose)
            {
                ClosePanel(panelType);
            }
        }

        /// <summary>
        /// 获取已打开的面板
        /// </summary>
        /// <param name="panelType">面板类型</param>
        /// <returns>面板实例，如果未打开返回null</returns>
        public UIPanel GetOpenedPanel(UIPanelType panelType)
        {
            return _openedPanels.GetValueOrDefault(panelType);
        }

        /// <summary>
        /// 检查面板是否已打开
        /// </summary>
        /// <param name="panelType">面板类型</param>
        /// <returns>是否已打开</returns>
        public bool IsPanelOpened(UIPanelType panelType)
        {
            return _openedPanels.ContainsKey(panelType);
        }

        /// <summary>
        /// 面板关闭时的回调
        /// </summary>
        private void OnPanelClosed(UIPanel panel)
        {
            _openedPanels.Remove(panel.panelType);

            // 如果是销毁类型的面板，从缓存中移除并销毁
            if (UIConfig.PanelInfoDict.TryGetValue(panel.panelType, out UIPanelInfo panelInfo) && panelInfo.isDestroyOnClose)
            {
                _panelCache.Remove(panel.panelType);
                panel.DestroyPanel();
            }
        }

        /// <summary>
        /// 获取层级Transform
        /// </summary>
        private Transform GetLayerTransform(UILayer layer)
        {
            return layer switch
            {
                UILayer.Background => backgroundLayer,
                UILayer.Normal => normalLayer,
                UILayer.Top => topLayer,
                UILayer.System => systemLayer,
                _ => normalLayer
            };
        }

        /// <summary>
        /// 获取下一个排序顺序
        /// </summary>
        private int GetNextSortingOrder(UILayer layer)
        {
            _currentSortingOrder[layer]++;
            return _currentSortingOrder[layer];
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            foreach (var panel in _panelCache.Values)
            {
                if (panel != null)
                {
                    panel.DestroyPanel();
                }
            }
            _panelCache.Clear();
            UIResourceLoader.Instance.UnloadUnusedAssets();
        }
    }
}
