using UnityEngine;
using BezierSolution;

[RequireComponent(typeof(BezierWalkerWithSpeed))]
public class WalkerController : MonoBehaviour
{
    public float speed = 10;
    public float shiftMultiplier = 1.7f;
    public float lerp = .05f;

    private BezierWalkerWithSpeed walker;
    private float targetVelocity;

    private void Start()
    {
        walker = GetComponent<BezierWalkerWithSpeed>();
    }

    void Update()
    {
        targetVelocity = Input.GetKey(KeyCode.LeftShift) ? Input.GetAxis("Horizontal") * speed * shiftMultiplier : Input.GetAxis("Horizontal") * speed;

        walker.speed = Mathf.Lerp(walker.speed, targetVelocity, lerp);
    }
}
