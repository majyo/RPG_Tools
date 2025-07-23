using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 游戏UI面板示例
    /// </summary>
    public class GameUIPanel : UIPanel
    {
        [Header("游戏UI组件")]
        public Text healthText;
        public Slider healthBar;
        public Text scoreText;
        public Button menuButton;
        public Button inventoryButton;
        
        private int currentHealth = 100;
        private int maxHealth = 100;
        private int currentScore = 0;

        protected override void OnInit()
        {
            base.OnInit();
            
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
                
            if (inventoryButton != null)
                inventoryButton.onClick.AddListener(OnInventoryClicked);
        }

        protected override void OnOpen(object data = null)
        {
            base.OnOpen(data);
            
            // 如果有传入数据，可以在这里处理
            if (data is GameUIData gameData)
            {
                currentHealth = gameData.health;
                maxHealth = gameData.maxHealth;
                currentScore = gameData.score;
            }
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            // 更新血量显示
            if (healthText != null)
                healthText.text = $"{currentHealth}/{maxHealth}";
                
            if (healthBar != null)
                healthBar.value = (float)currentHealth / maxHealth;
                
            // 更新分数显示
            if (scoreText != null)
                scoreText.text = $"Score: {currentScore}";
        }

        private void OnMenuClicked()
        {
            UIManager.Instance.OpenPanel(UIPanelType.MainMenu);
        }

        private void OnInventoryClicked()
        {
            UIManager.Instance.OpenPanel(UIPanelType.Inventory);
        }

        /// <summary>
        /// 设置血量
        /// </summary>
        public void SetHealth(int health, int maxHealth)
        {
            this.currentHealth = health;
            this.maxHealth = maxHealth;
            UpdateUI();
        }

        /// <summary>
        /// 设置分数
        /// </summary>
        public void SetScore(int score)
        {
            this.currentScore = score;
            UpdateUI();
        }
    }

    /// <summary>
    /// 游戏UI数据类
    /// </summary>
    [System.Serializable]
    public class GameUIData
    {
        public int health;
        public int maxHealth;
        public int score;

        public GameUIData(int health, int maxHealth, int score)
        {
            this.health = health;
            this.maxHealth = maxHealth;
            this.score = score;
        }
    }
}
