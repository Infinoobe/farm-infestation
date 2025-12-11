using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        public TMP_Text zombiesCounter;
        public GameObject deadSplashScreen;
        public GameObject backpackPanel;
        public GameObject shopPanel;

        private void Start()
        {
            backpackPanel.SetActive(false);
            shopPanel.SetActive(false);
        }

        void Update()
        {
            var uiText = $"";
            
            if (GameState.Instance.IsDay())
            {
                uiText += $"Money: {GameState.Instance.money}\n";
            }

            uiText += $"HP: {GameState.Instance.Player.hitPoints}";

            if (GameState.Instance.IsNight())
            {
                uiText += $"\nZombies to kill: {GameState.Instance.GetZombiesToKill()}";
            }
            zombiesCounter.text = uiText;
            
            deadSplashScreen.SetActive(GameState.Instance.Player.IsDead);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchBackpack();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SwitchShop();
            }
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
    }
}
