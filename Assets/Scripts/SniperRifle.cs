using UnityEngine;
using Hertzole.GoldPlayer;

public class SniperRifle : PlayerGun
{
    private GameObject sniperOverlay;
    public float zoomFov = 7.5f;

    private float baseZoomFov;
    private int zoomLevel = 0;

    private void Start()
    {
        baseZoomFov = zoomFov;
        // Retrieve the overlay
        sniperOverlay = FindObjectOfType<UIManager>().SniperOverlay;
    }

    new void Update()
    {
        // Yada yada yada
        base.Update();

        // Update the zoomFOV accoring to the zoom level
        float zoomFraction = 1f / (float)((float)zoomLevel + 1);
        zoomFov = zoomFraction * baseZoomFov;

        // Alter zoom level with Q
        if (Input.GetKeyDown(KeyCode.Q) && atSights)
        {
            zoomLevel++;
            if(zoomLevel > 2) { zoomLevel = 0; }

            // Update UI
            FindObjectOfType<UIManager>().SetZoomLevelDisplay(zoomLevel);
        }
    }

    public override void AimModes()
    {
        UnityEngine.UI.Image crosshair = FindObjectOfType<GunManager>().crosshair;
        Sprite crosshairHip = FindObjectOfType<GunManager>().crosshairHip;
        Sprite crosshairAim = FindObjectOfType<GunManager>().crosshairAim;
        if (atSights)
        {
            accuracy = gun.accuracyAtSights;
            crosshair.sprite = crosshairAim;
            Camera.main.fieldOfView = Vector2.Lerp(Vector2.one * Camera.main.fieldOfView, new Vector2(zoomFov, zoomFov), Time.deltaTime * 15).x;

            FindObjectOfType<GoldPlayerController>().Camera.MouseSensitivity = new Vector2(2, 2) / (zoomLevel + 1);
            FindObjectOfType<GoldPlayerController>().Movement.CanRun = false;

            // Should the overlay be active?
            if(Camera.main.fieldOfView - zoomFov < 2)
            {
                sniperOverlay.SetActive(true);

                // Deactive model rendering
                gameObject.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
        else
        {
            accuracy = gun.accuracyAtHip;
            crosshair.sprite = crosshairHip;
            Camera.main.fieldOfView = Vector2.Lerp(Vector2.one * Camera.main.fieldOfView, new Vector2(80, 80), Time.deltaTime * 5).x;

            FindObjectOfType<GoldPlayerController>().Camera.MouseSensitivity = new Vector2(4, 4);
            FindObjectOfType<GoldPlayerController>().Movement.CanRun = true;

            // Reactivate the normal stuff
            sniperOverlay.SetActive(false);
            gameObject.GetComponentInChildren<Renderer>().enabled = true;
        }
    }
}
