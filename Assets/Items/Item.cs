using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite sprite;

    private void OnValidate()
    {
        if (sprite != null)
        {
            EditorGUIUtility.SetIconForObject(this, sprite.texture);
        }
    }
}
