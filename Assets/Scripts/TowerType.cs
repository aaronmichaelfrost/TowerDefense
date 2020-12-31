using UnityEngine;

[CreateAssetMenu]
public class TowerType : ScriptableObject
{
    [Tooltip("A TowerType identifier, make sure this is unique")]
    public int uniqueId;


    public string typeName;
    public GameObject prefab;


    [Tooltip("The amount of money it costs to place a unit")]
    public float placeCost;


    [Tooltip("The amount of skill points it costs to unlock this tower in order to place units")]
    public int unlockCost;


    [Tooltip("An array of the towers that unlocking this tower will open an unlock path to")]
    public TowerType[] towersAfter;


    [Tooltip("The sprite to show when the tower is able to be purchased and used")]
    public Sprite unlockedIcon;


    [Tooltip("The sprite to show when the tower is able to be unlocked")]
    public Sprite unlockableIcon;


    [Tooltip("The sprite to show when the tower is not yet able to be unlocked")]
    public Sprite unknownIcon;
}

