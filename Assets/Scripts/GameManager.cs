using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Make singleton instance
    public static GameManager current = null;
    private void Awake()
    {
        if (current)
            Destroy(this.gameObject);

        current = this;
    }

    
    public PlayerStats playerStats;




    // Temp
    private bool validTowerPosition = false;
    private bool wasValidTowerPosition = false;
    private GameObject temporaryTowerBlueprint = null;

    public bool placeTowerMode = false;
    public GameObject selectedTowerPrefab;





    private void Update()
    {

        if (placeTowerMode)
            PlaceTowerMode();
        else if (temporaryTowerBlueprint)
            Destroy(temporaryTowerBlueprint);

    }


    void PlaceTowerMode()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,  20000f, LevelManager.current.placeTowerMask, QueryTriggerInteraction.Ignore))
        {
            // Spawn an inactive tower
            if (!temporaryTowerBlueprint)
            {
                temporaryTowerBlueprint = Instantiate(selectedTowerPrefab, hit.point, Quaternion.Euler(0, transform.eulerAngles.y, 0), LevelManager.current.towersHolder);
                temporaryTowerBlueprint.name = "Pending Tower Placement";

                // Set the material of the tower to show if the position is valid
                temporaryTowerBlueprint.GetComponent<MeshRenderer>().material = AssetManager.current.towerPlacementMaterial;
            }

            temporaryTowerBlueprint.transform.position = Vector3.Lerp(temporaryTowerBlueprint.transform.position, hit.point, 1f);

            validTowerPosition = Physics.OverlapSphere(temporaryTowerBlueprint.transform.position, 
                temporaryTowerBlueprint.GetComponent<SphereCollider>().radius, LevelManager.current.placeTowerCollisionMask).Length == 1;


            // Toggle color of temporary tower 
            if (validTowerPosition && !wasValidTowerPosition)
            {
                temporaryTowerBlueprint.GetComponent<MeshRenderer>().material.SetColor("Color_EFD058D7", Color.blue);
            }
            else if (!validTowerPosition && wasValidTowerPosition)
            {
                temporaryTowerBlueprint.GetComponent<MeshRenderer>().material.SetColor("Color_EFD058D7", Color.red);
            }
            


            if (Input.GetMouseButtonDown(0) && validTowerPosition)
                PlaceTower();

            wasValidTowerPosition = validTowerPosition;
        }
        else
        {
            if (temporaryTowerBlueprint)
                Destroy(temporaryTowerBlueprint);
        }
    }


    void PlaceTower()
    {
        GameObject placedTower = Instantiate(selectedTowerPrefab, temporaryTowerBlueprint.transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0), LevelManager.current.towersHolder);
        placedTower.GetComponent<Tower>().active = true;
    }


    public void TakeDamage(float amount)
    {
        playerStats.health -= amount;
    }


}
