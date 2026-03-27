using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem")]
public class ItemsDatabaseSO : ScriptableObject
{
    public List<Item> items;
}
