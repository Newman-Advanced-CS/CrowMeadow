using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleBase : MonoBehaviour
{
    [Header("Base Vehicle Parameters")]
    public Camera vehicleCam;
    public Rigidbody rb;

    Transform player;
    bool active = false;

    void Start()
    {
        vehicleCam.gameObject.SetActive(false);
    }

    public virtual void Update()
    {
        if (FindObjectOfType<GunManager>() != null && player == null)
            player = FindObjectOfType<GunManager>().transform;

        if (active)
        {
            ActiveUpdate();
        }

        // Check for activity
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (player == null)
                return;

            switch (active)
            {
                case false:
                    // Is the player within bounds
                    if (Vector3.Distance(player.position, transform.position) <= 40)
                    {
                        // ...and is looking at the vehicle
                        RaycastHit hit;

                        // Layer mask the raycast
                        int layerMask = 1 << 9;
                        layerMask = ~layerMask;

                        if (Physics.Raycast(player.position, Camera.main.transform.forward, out hit, 40, layerMask))
                        {
                            if (hit.transform == transform)
                            {
                                active = true;
                                player.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case true:
                    // Get out of the vehicle
                    active = false;
                    vehicleCam.gameObject.SetActive(false);
                    player.gameObject.SetActive(true);
                    player.transform.position = transform.position + Vector3.up * 5;
                    break;

            }
        }
    }

    public virtual void ActiveUpdate()
    {
        vehicleCam.gameObject.SetActive(true);

        // Look at object
        vehicleCam.transform.LookAt(transform.position);
    }
}
