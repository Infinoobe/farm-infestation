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
            topRight.text = $"{GameState.Instance.money} $";

            if (GameState.Instance.IsNight())
            {
                bottomLeft.text = $"Zombies to kill: {GameState.Instance.GetZombiesToKill()}";
            }
            else
            {
                bottomLeft.text = $"";
            }
            
            hpLabel.text = $"HP: {GameState.Instance.Player.hitPoints}";

            var barWidth = 600.0f * GameState.Instance.Player.hitPoints / GameState.Instance.Player.hitPointsMax;
            hpImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barWidth);
            
            deadSplashScreen.SetActive(GameState.Instance.Player.IsDead);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SwitchBackpack()
        {
            backpackPanel.SetActive(!backpackPanel.activeSelf);
        }

        public void SwitchShop()
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }

        public void SwitchResearch()
        {
            researchPanel.SetActive(!researchPanel.activeSelf);
        }

        public bool IsBackpackVisible()
        {
            return backpackPanel.activeSelf;
        }
    }
}
