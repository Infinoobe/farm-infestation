using System;
using IngameDebugConsole;
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

        [ConsoleMethod( "give_me_money", "Adds 999 $" )]
        public static void GiveCashCheat()
        {
            var gi = GameState.Instance;
            gi.AddItem(gi.itemsDatabase.moneyItemSo, 999);
        }
    
        [ConsoleMethod( "items_for_upgrades", "Adds inventory to buy all upgrades" )]
        public static void GiveStuffForUpgades()
        {
            var gi = GameState.Instance;
            var sns = FindObjectsByType<SkillNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var sn in sns)
            {
                var items_needed = sn.skillSO.GetRequiredItems().GetItemsOfTypeDict();
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

        [ConsoleMethod("god_toggle", "Toggles god mode")]
        public static  void ToggleGodCheat()
        {
            var gi = GameState.Instance;
            gi.GodCheat = !gi.GodCheat;
        }

        [ConsoleMethod("dmg_toggle", "Toggles max damage mode")]
        public static void ToggleDamageCheat()
        {
            var gi = GameState.Instance;
            gi.DamageCheat = !gi.DamageCheat;
        }

    }
}
