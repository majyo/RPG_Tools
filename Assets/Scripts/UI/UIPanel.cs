using System;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("UI面板配置")]
        public UIPanelType panelType = UIPanelType.None;
        public UILayer layer = UILayer.Normal;
        
        /// <summary>
        /// 面板是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// 面板是否正在显示
        /// </summary>
        public bool IsShowing { get; private set; }
        
        /// <summary>
        /// 面板打开完成事件
        /// </summary>
        public event Action<UIPanel> OnPanelOpened;
        
        /// <summary>
        /// 面板关闭完成事件
        /// </summary>
        public event Action<UIPanel> OnPanelClosed;

        protected virtual void Awake()
        {
            if (!IsInitialized)
            {
                OnInit();
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化面板（只会调用一次）
        /// </summary>
        protected virtual void OnInit()
        {
            // 子类可重写此方法进行初始化
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="data">传递给面板的数据</param>
        public virtual void Open(object data = null)
        {
            if (IsShowing) return;
            
            IsShowing = true;
            gameObject.SetActive(true);
            
            OnOpen(data);
            OnPanelOpened?.Invoke(this);
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public virtual void Close()
        {
            if (!IsShowing) return;
            
            IsShowing = false;
            
            OnClose();
            OnPanelClosed?.Invoke(this);
            
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 面板打开时调用
        /// </summary>
        /// <param name="data">传递的数据</param>
        protected virtual void OnOpen(object data = null)
        {
            // 子类可重写此方法
        }

        /// <summary>
        /// 面板关闭时调用
        /// </summary>
        protected virtual void OnClose()
        {
            // 子类可重写此方法
        }

        /// <summary>
        /// 销毁面板
        /// </summary>
        public virtual void DestroyPanel()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 设置面板层级
        /// </summary>
        /// <param name="sortingOrder">排序顺序</param>
        public void SetSortingOrder(int sortingOrder)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
            }
            canvas.sortingOrder = sortingOrder;
        }
    }
}
