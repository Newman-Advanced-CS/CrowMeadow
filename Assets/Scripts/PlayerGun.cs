using UnityEngine;
using System.Collections;
using Hertzole.GoldPlayer;

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Requirements")]
    public Transform shootingPos;
    public GameObject bulletPrefab;
    public GameObject muzzleFlash;
    public Animator anim;

    [Header("Gun Settings")]
    public Gun gun;
    [HideInInspector]
    public float accuracy;

    // Private vars
    private float timeAtLastShot = 0;
    private float baseRecoil;
    private float currentRecoil;

    [HideInInspector]
    public bool atSights;
    private int ammoInClip;
    private int ammoTotal;

    [HideInInspector]
    public bool canAimSights = true;

    [HideInInspector]
    public bool isReloading = false;

    private void Awake()
    {
        baseRecoil = gun.baseRecoilAmount;
        currentRecoil = baseRecoil;

        // Set up ammo count
        ammoInClip = gun.ammoPerMagazine;
        ammoTotal = ammoInClip * 10;
    }

    public void Update()
    {
        if(ShootInput())
        {
            // Calculate the amount of time that has passed
            float timeSinceLastShot = Time.time - timeAtLastShot;
            if(timeSinceLastShot >= gun.shootingInterval && ammoInClip > 0 && !isReloading)
            {
                timeAtLastShot = Time.time;

                // Shoot
                Shoot();
            }
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
        if(canAimSights)
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
        if(!isReloading)
        {
            if(!atSights)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
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

    // Check if the player is shooting
    bool ShootInput()
    {
        if(gun.constantFire)
        {
            return Input.GetMouseButton(0);
        }
        else
        {
            return Input.GetMouseButtonDown(0);
        }
    }

    void Shoot()
    {
        // Play sound
        AudioManager.Play(gun.shotClip, 1, 1, false);

        // Calculate offset for accuracy
        float xOffset = (100 - accuracy) * Random.Range(-0.05f, 0.05f);
        float yOffset = (100 - accuracy) * Random.Range(-0.05f, 0.05f);
        Vector3 offset = new Vector3(xOffset, yOffset, 0);
        
        // Create bullet & apply offset
        GameObject bullet = Instantiate(bulletPrefab, shootingPos.position, shootingPos.rotation);
        bullet.transform.eulerAngles += offset;
        bullet.GetComponent<Bullet>().damage = gun.damage;

        // Recoil
        FindObjectOfType<GoldPlayerController>().Camera.ApplyRecoil(currentRecoil, 0.6f);
        currentRecoil += 1.75f;

        // Max recoil cut off
        if (currentRecoil > 90f) { currentRecoil = 90;  }

        // Subtract ammo
        ammoInClip--;

        // Muzzle Flash
        GameObject flash = Instantiate(muzzleFlash, shootingPos.position, shootingPos.rotation);
        flash.transform.parent = Camera.main.transform.parent;
        flash.GetComponent<ParticleSystem>().Play();
    }

    void UpdateStats(PlayerStats statManager)
    {
        statManager.gunEquiped = gun.GunName;
        statManager.ammoInClip = ammoInClip;
        statManager.ammoInTotal = ammoTotal;
    }

    IEnumerator Reload()
    {
        atSights = false;
        AudioManager.Play("Reload", 1, 1, false);

        isReloading = true;
        yield return new WaitForSeconds(2.5f);
        isReloading = false;

        // Update ammo count
        int realAmmoTotal = ammoTotal + ammoInClip;
        int ammoNeeded = gun.ammoPerMagazine - ammoInClip;
        if(ammoNeeded > realAmmoTotal)
        {
            ammoInClip = realAmmoTotal;
            ammoTotal = 0;
        }
        else
        {
            ammoInClip = gun.ammoPerMagazine;
            ammoTotal -= ammoNeeded;
        }
    }
}
