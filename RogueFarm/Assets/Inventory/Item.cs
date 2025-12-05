using UnityEngine;

[CreateAssetMenu(fileName = "NewItem")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int value;
}