using UnityEngine;

public class Noclip : MonoBehaviour
{
    public bool Enabled = false;
    private bool inNoclip = false;

    private float X;
    private float Y;
    public float Sensitivity;
    private Camera cam;
    private Camera goldPlayerCam;

    public void Start()
    {
        if (!Enabled)
            Destroy(gameObject);

        Vector3 euler = transform.rotation.eulerAngles;
        X = euler.x;
        Y = euler.y;

        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    private void Update()
    {
        if(goldPlayerCam == null && FindObjectOfType<GunManager>() != null)
        {
            goldPlayerCam = FindObjectOfType<GunManager>().gameObject.GetComponentInChildren<Camera>();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            inNoclip = !inNoclip;
            gameObject.SetActive(inNoclip);
        }

        if(goldPlayerCam != null)
        {
            if (inNoclip)
            {
                goldPlayerCam.enabled = false;
                cam.enabled = true;

                // Mouse look I copied from stack overflow
                const float MIN_X = 0.0f;
                const float MAX_X = 360.0f;
                const float MIN_Y = -90.0f;
                const float MAX_Y = 90.0f;

                X += Input.GetAxis("Mouse X") * (Sensitivity * Time.deltaTime);
                if (X < MIN_X) X += MAX_X;
                else if (X > MAX_X) X -= MAX_X;
                Y -= Input.GetAxis("Mouse Y") * (Sensitivity * Time.deltaTime);
                if (Y < MIN_Y) Y = MIN_Y;
                else if (Y > MAX_Y) Y = MAX_Y;

                transform.rotation = Quaternion.Euler(Y, X, 0.0f);

                // Movement I made myself
                transform.position += transform.right * Input.GetAxis("Horizontal");
            }
            else
            {
                goldPlayerCam.enabled = true;
                cam.enabled = false;
            }
        }
    }
}
