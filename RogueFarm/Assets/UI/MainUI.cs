using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        public TMP_Text bottomLeft;
        public TMP_Text topRight;
        public TMP_Text hpLabel;
        public Image hpImage;

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
