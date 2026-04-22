using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CheatsUI : MonoBehaviour
    {
        public Toggle GodMode;
        public Toggle DamageMode;

        public void OnEnable()
        {
            var gi = GameState.Instance;
            GodMode.isOn = gi.GodCheat;
            DamageMode.isOn = gi.DamageCheat;
        }

        public void GiveCashCheat()
        {
            var gi = GameState.Instance;
            gi.AddItem(gi.moneyItemSo, 999);
        }
    
        public void GiveStuffForUpgades()
        {
            var gi = GameState.Instance;
            var sns = FindObjectsByType<SkillNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var sn in sns)
            {
                var items_needed = sn.skillSO.GetItemsDictionary();
                foreach (var (k, v) in items_needed)
                {
                    gi.AddItem(k, v);
                }
            
            }
        }

        public void UpdateToggles()
        {
            var gi = GameState.Instance;
            gi.GodCheat = GodMode.isOn;
            gi.DamageCheat = DamageMode.isOn;
        }

    }
}
