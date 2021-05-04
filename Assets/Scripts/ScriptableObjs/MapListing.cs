using UnityEngine;

[CreateAssetMenu(fileName = "NewMap", menuName = "ScriptableObjects/Map Listing")]
public class MapListing : ScriptableObject
{
    public string MapName;
    [TextArea(3, 10)]
    public string MapDescription;
    public Sprite Image;
    public int sceneIndex;
}
