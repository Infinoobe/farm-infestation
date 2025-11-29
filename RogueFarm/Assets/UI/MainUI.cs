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
            zombiesCounter.text = $"HP: {GameState.Instance.Player.hitPoints}\n"
                                  +$"Zombies to kill: {GameState.Instance.GetZombiesToKill()}";
        }
    }
}
