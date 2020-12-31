using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager current = null;

    private void Awake()
    {
        if (current)
            Destroy(this.gameObject);

        current = this;
    }


    public Material towerPlacementMaterial;

}
