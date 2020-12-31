using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BezierSolution.BezierWalkerWithSpeed))]
public class Enemy : MonoBehaviour
{

    /* Set in inspector */

    public float startHealth;
    public float speed;

    [Tooltip("The damage the enemy does to us when it's path is completed")]
    public float damage;

    [Tooltip("The amount of time it takes for the enemy to be deleted after death")]
    public float fadeOutTime = 5f;

    [Tooltip("I know this is kind of gross to have, but its for the tracking for homing bullets since they need a transform to follow")]
    public Transform centerOfBody;

    /*********************/




    // Temporary

    [System.NonSerialized]
    public bool alive = true;

    [System.NonSerialized]
    public float health;

    [System.NonSerialized]
    public SkinnedMeshRenderer meshRenderer;

    [System.NonSerialized]
    public float distanceTravelled;








    // Needs implemented
    public enum EnemyClass
    {
        earth,
        spy,
        fire,
        ink,
        metal
    }

    public EnemyClass enemyClass;


    private BezierSolution.BezierWalkerWithSpeed walker;


    public void Update()
    {
        if(walker.enabled)
            distanceTravelled = walker.NormalizedT;
    }


    private void Awake()
    {
        walker = GetComponent<BezierSolution.BezierWalkerWithSpeed>();

        // Randomize speed
        speed *= Random.Range(.5f, 2f);

        walker.speed = speed;

        walker.onPathCompleted.AddListener(PathCompleted);

        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        health = startHealth;
    }

    void Start()
    {

        // Randomize color
        meshRenderer.material.SetColor("Tint", RandomHDRColor());

        // Randomize tiling of the damage overlay
        //meshRenderer.material.SetVector("DamageTiling", new Vector4(Random.Range(-1, 1), Random.Range(-1, 1), 0, 0));

    }

    public static Color RandomHDRColor()
    {
        return new Color(Random.Range(0, .5f) + .5f, Random.Range(0, .5f) + .5f, Random.Range(0, .5f) + .5f);
    }


    
    public void PathCompleted()
    {
        GameManager.current.TakeDamage(damage);
        LevelManager.current.enemies.Remove(this);
        Destroy(this.gameObject);
    }


    public void Ragdoll()
    {
        walker.enabled = false;
        GetComponentInChildren<Animator>().enabled = false;
    }


    public void Die()
    {
        alive = false;
        GetComponent<CapsuleCollider>().enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Bones");
        Ragdoll();

        LevelManager.current.enemies.Remove(this);

        StartCoroutine(FadeOut());

        Destroy(this.gameObject, fadeOutTime);
    }


    // Disintegrate using shader
    public IEnumerator FadeOut()
    {
        float timeElapsed = 0;

        while (timeElapsed < fadeOutTime)
        {
            timeElapsed += Time.deltaTime;

            meshRenderer.material.SetFloat("Fade", Mathf.Clamp(timeElapsed / fadeOutTime, 0, 1));

            yield return null;
        }

    }


    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Die();

        // Visualize damage through shader
        meshRenderer.material.SetFloat("Damage", Mathf.Clamp(1- (health / startHealth), 0, 1));
    }
}
