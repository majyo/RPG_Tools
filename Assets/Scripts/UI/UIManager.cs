using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI管理器 - 负责管理所有UI面板的开启、关闭和生命周期
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    instance = go.AddComponent<UIManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
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
        private Dictionary<UIPanelType, UIPanel> openedPanels = new Dictionary<UIPanelType, UIPanel>();

        /// <summary>
        /// 面板实例缓存
        /// </summary>
        private Dictionary<UIPanelType, UIPanel> panelCache = new Dictionary<UIPanelType, UIPanel>();

        /// <summary>
        /// 当前排序顺序
        /// </summary>
        private Dictionary<UILayer, int> currentSortingOrder = new Dictionary<UILayer, int>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeLayers();
            }
            else if (instance != this)
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
                GameObject canvasGO = new GameObject("UICanvas");
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
            currentSortingOrder[UILayer.Background] = (int)UILayer.Background;
            currentSortingOrder[UILayer.Normal] = (int)UILayer.Normal;
            currentSortingOrder[UILayer.Top] = (int)UILayer.Top;
            currentSortingOrder[UILayer.System] = (int)UILayer.System;
        }

        /// <summary>
        /// 创建层级节点
        /// </summary>
        private void CreateLayerIfNotExists(ref Transform layer, string layerName)
        {
            if (layer == null)
            {
                GameObject layerGO = new GameObject(layerName);
                layerGO.transform.SetParent(uiCanvas.transform, false);
                layer = layerGO.transform;

                // 添加RectTransform并设置为全屏
                RectTransform rect = layerGO.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
            }
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
            if (openedPanels.ContainsKey(panelType))
            {
                Debug.LogWarning($"Panel {panelType} is already opened");
                onComplete?.Invoke(openedPanels[panelType]);
                return;
            }

            // 获取面板配置信息
            if (!UIConfig.PanelInfoDict.TryGetValue(panelType, out UIPanelInfo panelInfo))
            {
                Debug.LogError($"Panel info not found for type: {panelType}");
                return;
            }

            // 检查缓存中是否有面板实例
            if (panelCache.TryGetValue(panelType, out UIPanel cachedPanel))
            {
                ShowPanel(cachedPanel, data, onComplete);
                return;
            }

            // 异步加载面板
            UIResourceLoader.Instance.LoadUIPrefabAsync(panelInfo.prefabPath, (GameObject prefabInstance) =>
            {
                if (prefabInstance == null)
                {
                    Debug.LogError($"Failed to load panel prefab: {panelInfo.prefabPath}");
                    return;
                }

                UIPanel panel = prefabInstance.GetComponent<UIPanel>();
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
                    panelCache[panelType] = panel;
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
            openedPanels[panel.panelType] = panel;
            panel.Open(data);
            onComplete?.Invoke(panel);
        }

        /// <summary>
        /// 关闭UI面板
        /// </summary>
        /// <param name="panelType">面板类型</param>
        public void ClosePanel(UIPanelType panelType)
        {
            if (openedPanels.TryGetValue(panelType, out UIPanel panel))
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
            List<UIPanelType> panelsToClose = new List<UIPanelType>(openedPanels.Keys);
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
            return openedPanels.TryGetValue(panelType, out UIPanel panel) ? panel : null;
        }

        /// <summary>
        /// 检查面板是否已打开
        /// </summary>
        /// <param name="panelType">面板类型</param>
        /// <returns>是否已打开</returns>
        public bool IsPanelOpened(UIPanelType panelType)
        {
            return openedPanels.ContainsKey(panelType);
        }

        /// <summary>
        /// 面板关闭时的回调
        /// </summary>
        private void OnPanelClosed(UIPanel panel)
        {
            openedPanels.Remove(panel.panelType);

            // 如果是销毁类型的面板，从缓存中移除并销毁
            if (UIConfig.PanelInfoDict.TryGetValue(panel.panelType, out UIPanelInfo panelInfo) && panelInfo.isDestroyOnClose)
            {
                panelCache.Remove(panel.panelType);
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
            currentSortingOrder[layer]++;
            return currentSortingOrder[layer];
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            foreach (var panel in panelCache.Values)
            {
                if (panel != null)
                {
                    panel.DestroyPanel();
                }
            }
            panelCache.Clear();
            UIResourceLoader.Instance.UnloadUnusedAssets();
        }
    }
}
