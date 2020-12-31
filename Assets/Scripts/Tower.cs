using UnityEngine;



public class Tower : MonoBehaviour
{


    /* Set these in the inspector ***************************************************/

    public bool debug = false;


    [Header("Stats")]
    [Space(20)]
    public float range; 
    public bool burst; // Needs Implemented
    public int burstAmount; // Needs Implemented
    public float primaryCooldown;
    public float secondaryCooldown; // Needs Implemented
    public float damage;
    public float projectileSpeed;

    public Ability ability;
    public ProjectileType projectileType;
    public DamageType damageType;
    public TargetingMode targetingMode;

    [Header("References")]
    [Space(20)]


    [Tooltip("The position at which to spawn projectiles")]
    public Transform projectileSpawn;

    [Tooltip("The GameObject that shoots out of this tower")]
    public GameObject projectilePrefab;

    [Tooltip("The layers that are considered enemies")]
    public LayerMask enemyMask;

    [Tooltip("The layers that this tower won't try to shoot through")]
    public LayerMask towerVisionBlockerMask;


    /* ********************************************************************************* */






    // Temp
    private float timeSinceLastAttack = 10000f;

    [System.NonSerialized]
    public bool active = false;

    private Enemy[] enemiesInRange;
    private int currentTargetIndex;

    [System.NonSerialized]
    public Enemy currentTarget;




    private void Update()
    {
        if (active)
        {
            timeSinceLastAttack += Time.deltaTime;
            UpdateEnemiesInRange();

            if (enemiesInRange != null)
            {
                UpdateCurrentTargetIndex();

                currentTarget = LevelManager.current.enemies[currentTargetIndex];

                if (timeSinceLastAttack > primaryCooldown)
                    Attack();
            }
            else
            {
                currentTarget = null;
            }

        }

    }


    // Draw the range
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }


    // Spawn a projectile and give it a starting direction
    private void InitializeProjectile(bool homing, Vector3 targetDirection)
    {
        Projectile p = Instantiate(projectilePrefab, projectileSpawn.position, 
            Quaternion.LookRotation(targetDirection), LevelManager.current.projectilesHolder).GetComponent<Projectile>();


        p.damage = damage;
        p.speed = projectileSpeed;
        p.targetEnemy = currentTarget;


        p.homing = homing;

        // Projectile handles target direction for homing on update, if set to homing, just need to pass in the tower for target swapping functionality
        if (homing)
            p.tower = this;
        else
            p.targetDirection = targetDirection;
    }





    private void ExecuteHomingAttack()
    {
 
        if(Physics.Linecast(projectileSpawn.position, currentTarget.centerOfBody.position, towerVisionBlockerMask))
        {

            // Debug red line to show that our vision was blocked
            if (debug)
                Debug.DrawLine(projectileSpawn.position, currentTarget.centerOfBody.position, Color.red, primaryCooldown);
        }
        else
        {
            if (debug)
                Debug.DrawLine(projectileSpawn.position, currentTarget.centerOfBody.position, Color.green, primaryCooldown);


            InitializeProjectile(true, Vector3.Normalize(currentTarget.centerOfBody.position - projectileSpawn.position));
        }
            
    }


    // Needs implemented
    private void ExecuteOmnidirectionalAttack()
    {
        Debug.Log("Executing Omnidirectional Attack");
    }


    // Needs implemented
    private void ExecuteRaycastAttack()
    {
        Debug.Log("Executing Raycast Attack");

        Enemy target = enemiesInRange[currentTargetIndex];
    }


    private void ExecuteLinearAttack()
    {
       

        // Get an array of samples spaced apart a certain distance
        BezierSolution.BezierWalkerWithSpeed.WalkerPredictionSample[] samples = currentTarget.GetComponent<BezierSolution.BezierWalkerWithSpeed>().predictionSamples(80, .1f);

        // The allowed time window in which the bullet can miss the enemy
        float missTimeAllowance = .05f;

        // Move the samples up to be the center of the character
        for (int i = 0; i < samples.Length; i++)
            samples[i].position.y += currentTarget.centerOfBody.localPosition.y;


        
        foreach (var samplePoint in samples)
        {
           
            if(Vector3.Distance(transform.position, samplePoint.position) < range)
            {

                

                // If the bullet can reach the sample point within the miss time window (missTimeAllowance)
                if (Mathf.Abs(Vector3.Distance(projectileSpawn.position, samplePoint.position) / projectileSpeed - samplePoint.time) < missTimeAllowance)
                {


                    if (Physics.Linecast(projectileSpawn.position, samplePoint.position, towerVisionBlockerMask))
                    {
                        // Debug red line to show the sample was blocked
                        if (debug)
                            Debug.DrawLine(projectileSpawn.position, samplePoint.position, Color.red, primaryCooldown);
                    }
                    else
                    {

                        InitializeProjectile(false, Vector3.Normalize(samplePoint.position - projectileSpawn.position));

                        // Debug green line to show the sample was good
                        if (debug)
                            Debug.DrawLine(projectileSpawn.position, samplePoint.position, Color.green, primaryCooldown);

                        return;
                    }
                }
                else
                {
                    // Draw white line to show the sample was bad
                    if (debug)
                    {
                        if (Physics.Linecast(projectileSpawn.position, samplePoint.position, towerVisionBlockerMask))
                            Debug.DrawLine(projectileSpawn.position, samplePoint.position, Color.red, 1f);
                        else
                            Debug.DrawLine(projectileSpawn.position, samplePoint.position, Color.white, 1f);
                    }

                }
            }

        }


        // If the code reaches here, it means that none of the samples could reach the enemy in time or were out of range or were blocked by an object

        // If there is an enemy farther back, target him instead, this is recursive and thus can go back as many enemies as neccecary
        if (currentTargetIndex - 1 > -1)
        {
            currentTarget = LevelManager.current.enemies[--currentTargetIndex];
            ExecuteLinearAttack();
        } 
    }


   

    // This function decides which attack to execute based on the tower's projectile type
    private void Attack()
    {

        timeSinceLastAttack = 0;
        switch (projectileType)
        {
            case ProjectileType.homing:
                ExecuteHomingAttack();
                break;
            case ProjectileType.omnidirectional:
                ExecuteOmnidirectionalAttack();
                break;
            case ProjectileType.raycast:
                ExecuteRaycastAttack();
                break;
            case ProjectileType.linear:
                ExecuteLinearAttack();
                break;
            default:
                break;
        }
    }


    // This function updates the enemiesInRange array
    private void UpdateEnemiesInRange()
    {

        // Create an array of the enemies in range
        Collider[] enemyColsInRange = Physics.OverlapSphere(transform.position, range, enemyMask, QueryTriggerInteraction.Ignore);

        enemiesInRange = new Enemy[enemyColsInRange.Length];

        if (enemyColsInRange.Length > 0)
        {
            for (int i = 0; i < enemyColsInRange.Length; i++)
                enemiesInRange[i] = enemyColsInRange[i].GetComponent<Enemy>();
        }
        else
            enemiesInRange = null;


        if (enemiesInRange != null)
            GetComponentInChildren<TMPro.TextMeshPro>().text = enemiesInRange.Length.ToString();
        else
            GetComponentInChildren<TMPro.TextMeshPro>().text = "0";
    }


    // This function updates the currentTargetIndex based on the tower's targeting mode
    private void UpdateCurrentTargetIndex()
    {
        switch (targetingMode)
        {
            case TargetingMode.last:

                int lowestIndex  = 1000000;
                foreach (var enemy in enemiesInRange)
                {
                    if (LevelManager.current.enemies.IndexOf(enemy) < lowestIndex)
                        lowestIndex = LevelManager.current.enemies.IndexOf(enemy);
                }
                currentTargetIndex = lowestIndex;

                return;
            case TargetingMode.first:

                int highestIndex = -1;
                foreach (var enemy in enemiesInRange)
                {
                    if (LevelManager.current.enemies.IndexOf(enemy) > highestIndex)
                        highestIndex = LevelManager.current.enemies.IndexOf(enemy);
                }
                currentTargetIndex = highestIndex;

                return;
            case TargetingMode.highestHealth:

                int highestHealthIndex = 0;
                float highestHealth = 0;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy.health > highestHealth)
                    {
                        highestHealth = enemy.health;
                        highestHealthIndex = LevelManager.current.enemies.IndexOf(enemy);
                    }
                }
                currentTargetIndex = highestHealthIndex;

                return;
            case TargetingMode.closest:

                int closestIndex = 0;
                float shortestDistance = 100000;
                foreach (var enemy in enemiesInRange)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestIndex = LevelManager.current.enemies.IndexOf(enemy);
                    }
                }
                currentTargetIndex = closestIndex;

                return;
            default:
                break;
        }

    }




    public enum ProjectileType
    {
        homing,
        omnidirectional, // Needs implemented
        raycast, // Needs implemented
        linear 
    }


    // Needs implemented
    public enum DamageType
    {
        bleed,
        instant
    }


    // Needs implemented
    public enum Ability
    {
        fire,
        water,
        metal
    }


    public enum TargetingMode
    {
        first, 
        last,
        highestHealth, // Automatically do
        closest 
    }
}
