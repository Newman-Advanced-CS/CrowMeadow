using UnityEngine;
using Hertzole.GoldPlayer;

public class PlayerGun : GunObject
{
    public Animator anim;

    [HideInInspector]
    public float baseRecoil;
    [HideInInspector]
    public float currentRecoil;

    public override void Awake()
    {
        base.Awake();

        baseRecoil = gun.baseRecoilAmount;
        currentRecoil = baseRecoil;
    }

    public void Update()
    {
        if (ShootInput())
        {
            // Shoot
            Shoot();
        }
        else
        {
            currentRecoil = baseRecoil;
        }

        // Update UI Stats
        UpdateStats(FindObjectOfType<PlayerStats>());

        // Check if aiming at sights
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))
        {
            if (!isReloading && canAimSights) { atSights = !atSights; } else { atSights = false; }
        }

        // Do aiming modes
        if (canAimSights)
        {
            AimModes();
        }

        // Update animation state
        anim.SetBool("atSights", atSights);
        anim.SetBool("isReloading", isReloading);

        // Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        // Point at whatever is ahead
        if (!isReloading)
        {
            if (!atSights)
            {
                RaycastHit hit;

                // Layer mask the raycast
                int layerMask = 1 << 9;
                layerMask = ~layerMask;

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10, layerMask))
                {
                    // Very strange look at script 
                    Quaternion OriginalRot = transform.rotation;
                    transform.LookAt(hit.point);
                    Quaternion NewRot = transform.rotation;
                    transform.rotation = OriginalRot;
                    transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, Time.deltaTime * 20);
                }
            }
            else
            {
                transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    void UpdateStats(PlayerStats statManager)
    {
        statManager.gunEquiped = gun.GunName;
        statManager.ammoInClip = ammoInClip;
        statManager.ammoInTotal = ammoTotal;
    }

    // Check if the player is shooting
    bool ShootInput()
    {
        if (gun.constantFire)
        {
            return Input.GetMouseButton(0);
        }
        else
        {
            return Input.GetMouseButtonDown(0);
        }
    }

    public virtual void AimModes()
    {
        UnityEngine.UI.Image crosshair = FindObjectOfType<GunManager>().crosshair;
        Sprite crosshairHip = FindObjectOfType<GunManager>().crosshairHip;
        Sprite crosshairAim = FindObjectOfType<GunManager>().crosshairAim;
        if (atSights)
        {
            accuracy = gun.accuracyAtSights;
            crosshair.sprite = crosshairAim;
            Camera.main.fieldOfView = Vector2.Lerp(Vector2.one * Camera.main.fieldOfView, new Vector2(60, 60), Time.deltaTime * 5).x;

            FindObjectOfType<GoldPlayerController>().Camera.MouseSensitivity = new Vector2(2, 2);
            FindObjectOfType<GoldPlayerController>().Movement.CanRun = false;
        }
        else
        {
            accuracy = gun.accuracyAtHip;
            crosshair.sprite = crosshairHip;
            Camera.main.fieldOfView = Vector2.Lerp(Vector2.one * Camera.main.fieldOfView, new Vector2(80, 80), Time.deltaTime * 5).x;

            FindObjectOfType<GoldPlayerController>().Camera.MouseSensitivity = new Vector2(4, 4);
            FindObjectOfType<GoldPlayerController>().Movement.CanRun = true;
        }
    }

    public override void Shoot()
    {
        if(CanShoot())
        {
            base.Shoot();

            // Recoil
            FindObjectOfType<GoldPlayerController>().Camera.ApplyRecoil(currentRecoil, 0.6f);
            currentRecoil += 1.75f;

            // Max recoil cut off
            if (currentRecoil > 30f) { currentRecoil = 30; }
        }
    }
}