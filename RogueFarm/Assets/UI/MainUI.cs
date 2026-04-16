using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        public static MainUI Instance;
        
        public TMP_Text bottomLeft;
        public TMP_Text topRight;
        public TMP_Text hpLabel;
        public Image hpImage;

        public GameObject deadSplashScreen;
        public GameObject backpackPanel;
        public GameObject shopPanel;
        public GameObject researchPanel;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            backpackPanel.SetActive(false);
            shopPanel.SetActive(false);
            researchPanel.SetActive(false);
        }

        void Update()
        {
            topRight.text = $"{GameState.Instance.GetMoney()} $";

            if (GameState.Instance.IsNight())
            {
                bottomLeft.text = $"Zombies to kill: {GameState.Instance.GetZombiesToKill()}";
            }
            else
            {
                bottomLeft.text = $"";
            }
            
            hpLabel.text = $"HP: {GameState.Instance.Player.hitPoints}";

            var barWidth = 600.0f * GameState.Instance.Player.hitPoints / (GameState.Instance.Player.hitPointsMax + GameState.Instance.MaxHealthBonusValue());
            hpImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barWidth);
            
            deadSplashScreen.SetActive(GameState.Instance.Player.IsDead);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ShowBackpack()
        {
            backpackPanel.SetActive(true);
            shopPanel.SetActive(false);
            researchPanel.SetActive(false);
        }

        public void HideBackpack()
        {
            backpackPanel.SetActive(false);
        }

        public void OpenShop()
        {
            shopPanel.SetActive(true);
            backpackPanel.SetActive(false);
            researchPanel.SetActive(false);
        }

        public void OpenResearch()
        {
            researchPanel.SetActive(true);
            backpackPanel.SetActive(false);
            shopPanel.SetActive(false);
        }

        public void CloseAllPanels()
        {
            backpackPanel.SetActive(false);
            researchPanel.SetActive(false);
            shopPanel.SetActive(false);
        }

    }
}
