using System.Collections;
using UnityEngine;

// Base class for all guns
public class GunObject : MonoBehaviour
{
    [Header("Gun Requirements")]
    public Transform shootingPos;
    public GameObject bulletPrefab;
    public GameObject muzzleFlash;

    [Header("Gun Settings")]
    public Gun gun;
    [HideInInspector]
    public float accuracy;

    // Private vars
    [HideInInspector]
    public float timeAtLastShot = 0;

    [HideInInspector]
    public bool atSights;
    [HideInInspector]
    public int ammoInClip;
    [HideInInspector]
    public int ammoTotal;

    [HideInInspector]
    public bool canAimSights = true;

    [HideInInspector]
    public bool isReloading = false;

    public virtual void Awake()
    {
        // Set up ammo count
        ammoInClip = gun.ammoPerMagazine;
        ammoTotal = ammoInClip * 10;
    }

    public bool CanShoot()
    {
        // Calculate the amount of time that has passed
        float timeSinceLastShot = Time.time - timeAtLastShot;
        return timeSinceLastShot >= gun.shootingInterval && ammoInClip > 0 && !isReloading;
    }

    public virtual void Shoot()
    {
        timeAtLastShot = Time.time;

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

        // Subtract ammo
        ammoInClip--;

        // Muzzle Flash
        GameObject flash = Instantiate(muzzleFlash, shootingPos.position, shootingPos.rotation);
        flash.transform.parent = transform.parent;
        flash.GetComponent<ParticleSystem>().Play();
    }

    public IEnumerator Reload()
    {
        atSights = false;
        AudioManager.Play("Reload", 1, 1, false);

        isReloading = true;
        yield return new WaitForSeconds(2.5f);
        isReloading = false;

        // Update ammo count
        int realAmmoTotal = ammoTotal + ammoInClip;
        int ammoNeeded = gun.ammoPerMagazine - ammoInClip;
        if (ammoNeeded > realAmmoTotal)
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
