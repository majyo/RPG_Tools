using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 对话框面板
    /// </summary>
    public class DialogPanel : UIPanel
    {
        [Header("对话框UI组件")]
        public Text titleText;
        public Text messageText;
        public Button confirmButton;
        public Button cancelButton;
        public Button closeButton;
        
        private Action onConfirm;
        private Action onCancel;

        protected override void OnInit()
        {
            base.OnInit();
            
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
                
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
                
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }

        protected override void OnOpen(object data = null)
        {
            base.OnOpen(data);
            
            if (data is DialogData dialogData)
            {
                SetupDialog(dialogData);
            }
        }

        private void SetupDialog(DialogData data)
        {
            // 设置标题
            if (titleText != null)
                titleText.text = data.title;
                
            // 设置消息内容
            if (messageText != null)
                messageText.text = data.message;
                
            // 设置按钮显示状态
            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(data.showConfirmButton);
                if (data.showConfirmButton && !string.IsNullOrEmpty(data.confirmButtonText))
                {
                    Text confirmText = confirmButton.GetComponentInChildren<Text>();
                    if (confirmText != null)
                        confirmText.text = data.confirmButtonText;
                }
            }
                
            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(data.showCancelButton);
                if (data.showCancelButton && !string.IsNullOrEmpty(data.cancelButtonText))
                {
                    Text cancelText = cancelButton.GetComponentInChildren<Text>();
                    if (cancelText != null)
                        cancelText.text = data.cancelButtonText;
                }
            }
                
            if (closeButton != null)
                closeButton.gameObject.SetActive(data.showCloseButton);
                
            // 保存回调函数
            onConfirm = data.onConfirm;
            onCancel = data.onCancel;
        }

        private void OnConfirmClicked()
        {
            onConfirm?.Invoke();
            UIManager.Instance.ClosePanel(UIPanelType.Dialog);
        }

        private void OnCancelClicked()
        {
            onCancel?.Invoke();
            UIManager.Instance.ClosePanel(UIPanelType.Dialog);
        }

        private void OnCloseClicked()
        {
            UIManager.Instance.ClosePanel(UIPanelType.Dialog);
        }

        /// <summary>
        /// 显示简单消息对话框
        /// </summary>
        public static void ShowMessage(string title, string message, Action onConfirm = null)
        {
            var dialogData = new DialogData
            {
                title = title,
                message = message,
                showConfirmButton = true,
                showCancelButton = false,
                showCloseButton = false,
                confirmButtonText = "确定",
                onConfirm = onConfirm
            };
            
            UIManager.Instance.OpenPanel(UIPanelType.Dialog, dialogData);
        }

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        public static void ShowConfirm(string title, string message, Action onConfirm = null, Action onCancel = null)
        {
            var dialogData = new DialogData
            {
                title = title,
                message = message,
                showConfirmButton = true,
                showCancelButton = true,
                showCloseButton = false,
                confirmButtonText = "确定",
                cancelButtonText = "取消",
                onConfirm = onConfirm,
                onCancel = onCancel
            };
            
            UIManager.Instance.OpenPanel(UIPanelType.Dialog, dialogData);
        }
    }

    /// <summary>
    /// 对话框数据类
    /// </summary>
    [System.Serializable]
    public class DialogData
    {
        public string title = "提示";
        public string message = "";
        public bool showConfirmButton = true;
        public bool showCancelButton = false;
        public bool showCloseButton = true;
        public string confirmButtonText = "确定";
        public string cancelButtonText = "取消";
        public Action onConfirm;
        public Action onCancel;
    }
}
