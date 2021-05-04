using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    public float distanceToPickup = 5f;
    public string soundEffect;

    Transform player;
    bool inZone = false;

    private void Update()
    {
        PlayerGun pRoot = FindObjectOfType<PlayerGun>();
        if(pRoot != null)
        {
            player = pRoot.transform;
            if (Vector3.Distance(player.position, transform.position) <= distanceToPickup)
            {
                if (!inZone)
                {
                    AudioManager.Play(soundEffect, 1, 1, false);
                    pRoot.ammoTotal += pRoot.gun.ammoPerMagazine;
                    FindObjectOfType<PlayerStats>().ammoInTotal = pRoot.ammoTotal;
                    Destroy(gameObject);
                }
                inZone = true;
            }
            else
            {
                inZone = false;
            }
        }
    }
}
