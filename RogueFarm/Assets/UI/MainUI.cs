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

        private void Start()
        {
            backpackPanel.SetActive(false);
        }

        void Update()
        {
            var uiText = $"HP: {GameState.Instance.Player.hitPoints}";
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
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SwitchBackpack()
        {
            backpackPanel.SetActive(!backpackPanel.activeSelf);
        }
    }
}
