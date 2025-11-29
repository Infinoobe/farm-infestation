using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        public TMP_Text zombiesCounter;
        public GameObject deadSplashScreen;
        void Update()
        {
            zombiesCounter.enabled = GameState.Instance.IsNight();
            zombiesCounter.text = $"HP: {GameState.Instance.Player.hitPoints}\n"
                                  +$"Zombies to kill: {GameState.Instance.GetZombiesToKill()}";
            deadSplashScreen.SetActive(GameState.Instance.Player.IsDead);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
