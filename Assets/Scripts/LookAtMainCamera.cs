using UnityEngine;


public class LookAtMainCamera : MonoBehaviour
{

    void Update()
    {
        transform.LookAt(Camera.main.transform);

        transform.Rotate(Vector3.up, 180);
        
    }
}
