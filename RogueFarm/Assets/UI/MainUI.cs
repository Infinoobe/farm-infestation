using TMPro;
using UnityEngine;

namespace UI
{
    public class MainUI : MonoBehaviour
    {
        public TMP_Text zombiesCounter;
        void Update()
        {
            zombiesCounter.enabled = GameState.Instance.IsNight();
            zombiesCounter.text = $"Zombies to kill: {GameState.Instance.GetZombiesToKill()}";
        }
    }
}
