using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    /* Set in inspector */

    public float maxLifeTime = 5;
    public bool explode = false;
    public float explosionForce;
    public float explosionRadius;

    [Range(100, 200)]
    public float turnSpeed = 120;

    /*********************/



    // Below are variables that are set by and dependent on the tower //

    [System.NonSerialized]
    public float speed;

    [System.NonSerialized]
    public float damage;

    [System.NonSerialized]
    public Enemy targetEnemy;

    [System.NonSerialized]
    public Tower tower;

    [System.NonSerialized]
    public bool homing = false;

    [System.NonSerialized]
    public Vector3 targetDirection;


    // ************************************ //




    // Temporary variables //

    private float currentLifeTime;

    private bool hasHit = false;






    private void Update()
    {
        // Update target direction if it is a homing projectile
        if (homing)
        {
            if (targetEnemy && targetEnemy.alive)
            {
                // Turn towards enemy

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetEnemy.centerOfBody.position - transform.position), .05f * Time.deltaTime * turnSpeed);

            }
            else
            {
                // Target new enemy if possible
                bool inRangeOfTower = Vector3.Distance(transform.position, tower.transform.position) < tower.range;

                if (tower && tower.currentTarget && inRangeOfTower)
                    targetEnemy = tower.currentTarget;

            }

            targetDirection = transform.forward;
        }


        // Travel in target direction
        transform.position += targetDirection.normalized * speed * Time.deltaTime;


        // Destroy if it's been alive for too long
        currentLifeTime += Time.deltaTime;

        if(currentLifeTime > maxLifeTime)
            Destroy(this.gameObject);

    }


    // Performs explosion physics
    private void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0.3F);
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (!hasHit)
        {
            hasHit = true;

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            

            if (explode)
                Explode();
        }

        Destroy(this.gameObject);

    }

}
