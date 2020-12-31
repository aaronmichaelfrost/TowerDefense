using UnityEngine;





// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
/* Attach this component to your camera
/* Prerequisite:
/* - Must have a parent transform to use as the orbital origin
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */





[RequireComponent(typeof(Camera))]
public class OrbitalCameraController : MonoBehaviour
{

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //
    // Set these in inspector

    public float minZoomDistance = 3f;
    public float maxZoomDistance = 8f;

    [Range(0, 35)]
    public float upperLookClampXEuler = 35f;

    [Range(260, 350)]
    public float lowerLookClampXEuler = 260f;

    [Range(.1f, 3)]
    public float lookSens = 1f;

    [Range(0, 1)]
    public float zoomSmoothness = .05f;

    [Range(40, 100)]
    public float zoomSpeed = 60f;
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ //



    // Temporary
    private float targetZoomDistance; 
    private Transform emptyTransform; // This is for the rotation sampling


    private void Awake()
    {
        Debug.Assert(transform.parent && transform.parent.name == "Orbital Camera Origin", "Orbital Camera must have parent named \"Orbital Camera Origin\".");
    }


    void Start() {

        targetZoomDistance = Vector3.Distance(transform.position, transform.parent.position);
        emptyTransform = Instantiate(new GameObject(), transform).transform;
    }


    void LateUpdate()
    {
        transform.LookAt(transform.parent);

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");


        // Use zoom input to adjust the desired zoom
        targetZoomDistance = zoomInput > 0 ? Mathf.Clamp(targetZoomDistance - zoomSpeed * Time.deltaTime, minZoomDistance, maxZoomDistance) : (zoomInput < 0 ? Mathf.Clamp(targetZoomDistance + zoomSpeed * Time.deltaTime, minZoomDistance, maxZoomDistance) : targetZoomDistance);

        Vector3 cameraOffsetDirection = Vector3.Normalize(transform.position - transform.parent.position);

        // Smoothly interpolate the distance of the camera from its parent
        transform.position = Vector3.Lerp(transform.position, transform.parent.position + cameraOffsetDirection * targetZoomDistance, zoomSmoothness);


        // Confined orbital rotation

        /*
         * Note that the sampling method performed here to check if the desired rotation is within the confines 
         * is really not ideal. Try to fix this later on.
         *
         * 
         */

        if (Input.GetKey(KeyCode.Mouse1))
        {
            
            transform.parent.Rotate(Vector3.up, Input.GetAxis("Mouse X") * lookSens * Time.deltaTime * 120, Space.World);

            emptyTransform.rotation = transform.parent.rotation;

            // Lock the speed at which we can look up and down to assist in the 
            float mouseY = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -3, 3);

            // Sample the rotation to see if it stays within confines
            // Dividing the looksens here can smooth out when the orbit reaches the confines
            emptyTransform.Rotate(Vector3.right, mouseY * lookSens / 2f * Time.deltaTime * 120, Space.Self);
            //emptyTransform.Rotate(Vector3.right, mouseY * lookSens, Space.Self);

            float x = emptyTransform.rotation.eulerAngles.x;

            if ((x < upperLookClampXEuler || x > lowerLookClampXEuler) || (x < lowerLookClampXEuler && x > 250 && mouseY > 0) || (x > upperLookClampXEuler && x < 250 && mouseY < 0) )
                transform.parent.Rotate(Vector3.right, mouseY * lookSens * Time.deltaTime * 120, Space.Self);

        }
    }
}