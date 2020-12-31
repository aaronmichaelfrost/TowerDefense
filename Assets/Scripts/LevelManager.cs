using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    // Make singleton instance
    public static LevelManager current = null;

    private void Awake()
    {
        if (current)
            Destroy(this.gameObject);

        current = this;
    }


    /* Level specific variables */
        /* Set these in inspector */

    [Tooltip("All of the different enemies that can spawn")]
    public List<GameObject> enemyPrefabs;


    [Tooltip("A transform parent container at which to insert new enemies under")]
    public Transform enemiesHolder;


    [Tooltip("A transform parent container at which to insert towers under")]
    public Transform towersHolder;


    [Tooltip("A transform parent container at which to insert projectiles under")]
    public Transform projectilesHolder;

    [Tooltip("The layers you can place towers on")]
    public LayerMask placeTowerMask;


    [Tooltip("The layers that the tower collider must not be colliding with in order to place the tower")]
    public LayerMask placeTowerCollisionMask;


    public TowerType[] towerTypes;


    /*****************************/





    // Temporary variables
    [System.NonSerialized]
    public List<Enemy> enemies = new List<Enemy>(); // The list of current enemies -> this gets sorted from shortest -> longest distance travelled

    private float timeSinceLastSpawn = 100000000; // The time since the last enemy was spawned





    private void Update()
    {
        SpawnRandomEnemiesConstant(.5f);

        OrderEnemies();
    }


    // Comparison class used for sorting: good for sorting by distance travelled in desceneding order
    class EnemyComparer : IComparer<Enemy>
    {
        public int Compare(Enemy x, Enemy y)
        {
            return x.distanceTravelled > y.distanceTravelled ? 1 : -1;
        }
    }

    private void OrderEnemies()
    {
        EnemyComparer ec = new EnemyComparer();
        enemies.Sort(ec);
    }


    public void PauseGame()
    {

        // Set time scale to 0


    }


    public void UnpauseGame()
    {

        // Set time scale to 1


    }


    private void SpawnRandomEnemiesConstant(float cooldown)
    {
        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn > cooldown)
        {
            timeSinceLastSpawn = 0;
            Enemy e = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count - 1)], enemiesHolder).GetComponent<Enemy>();
            enemies.Add(e);

        }

    }

}
