using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPGTools.UI.Panels
{
    /// <summary>
    /// 加载面板
    /// </summary>
    public class LoadingPanel : UIPanel
    {
        [Header("加载UI组件")]
        public Slider progressBar;
        public Text loadingText;
        public Text progressText;
        public Image loadingIcon;
        
        [Header("动画设置")]
        public float rotationSpeed = 90f;
        
        private Coroutine rotationCoroutine;

        protected override void OnInit()
        {
            base.OnInit();
            
            if (progressBar != null)
                progressBar.value = 0f;
        }

        protected override void OnOpen(object data = null)
        {
            base.OnOpen(data);
            
            if (data is LoadingData loadingData)
            {
                if (loadingText != null)
                    loadingText.text = loadingData.loadingMessage;
            }
            else
            {
                if (loadingText != null)
                    loadingText.text = "Loading...";
            }
            
            StartLoadingAnimation();
        }

        protected override void OnClose()
        {
            base.OnClose();
            StopLoadingAnimation();
        }

        private void StartLoadingAnimation()
        {
            if (loadingIcon != null)
            {
                rotationCoroutine = StartCoroutine(RotateIcon());
            }
        }

        private void StopLoadingAnimation()
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
                rotationCoroutine = null;
            }
        }

        private IEnumerator RotateIcon()
        {
            while (true)
            {
                loadingIcon.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// 更新加载进度
        /// </summary>
        /// <param name="progress">进度 (0-1)</param>
        /// <param name="message">进度消息</param>
        public void UpdateProgress(float progress, string message = "")
        {
            if (progressBar != null)
                progressBar.value = progress;
                
            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
                
            if (!string.IsNullOrEmpty(message) && loadingText != null)
                loadingText.text = message;
        }
    }

    /// <summary>
    /// 加载数据类
    /// </summary>
    [System.Serializable]
    public class LoadingData
    {
        public string loadingMessage;
        
        public LoadingData(string message)
        {
            this.loadingMessage = message;
        }
    }
}
