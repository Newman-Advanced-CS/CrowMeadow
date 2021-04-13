using UnityStandardAssets.Water;
using UnityEngine;
using Hertzole.GoldPlayer;

public class PlayerSwimming : MonoBehaviour
{
    Transform waterPlane;
    GoldPlayerController player;
    float initialGravity;
    GunManager gunManager;

    private void Start()
    {
        gunManager = FindObjectOfType<GunManager>();
        player = GetComponent<GoldPlayerController>();
        initialGravity = player.Movement.Gravity;

        // Check if there's water
        WaterBase water = FindObjectOfType<WaterBase>();
        if(water == null)
        {
            // Script is redundant, bye bye
            Destroy(this);
        }
        else
        {
            waterPlane = water.transform;
        }
    }

    private void Update()
    {
        // Are we under water?
        if (transform.position.y <= waterPlane.position.y)
        {
            // Disable aim at sights
            gunManager.SightsStatus(false);

            // Disable Gold Player Stuff
            player.Movement.CanRun = false;
            player.Movement.CanCrouch = false;
            player.Movement.Gravity = 0;
            player.Movement.EnableGroundStick = false;

            // Bob upwards to the surface.
            Vector3 bobPosition = transform.position;
            bobPosition.y += Time.deltaTime;
            if (bobPosition.y > waterPlane.position.y)
            {
                bobPosition.y = waterPlane.position.y;
            }
            transform.position = bobPosition;
        }
        else
        {
            // Enable aim at sights
            gunManager.SightsStatus(true);

            // Enable Gold Player Stuff
            player.Movement.CanRun = true;
            player.Movement.CanCrouch = true;
            player.Movement.Gravity = initialGravity;
            player.Movement.EnableGroundStick = true;
        }
    }
}
