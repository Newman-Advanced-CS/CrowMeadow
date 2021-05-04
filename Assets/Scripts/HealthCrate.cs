using UnityEngine;

public class HealthCrate : MonoBehaviour
{
    public float distanceToPickup = 5f;
    public string soundEffect;

    Transform player;
    bool inZone = false;
    bool enemyInZone = false;

    private void Update()
    {
        PlayerGun pRoot = FindObjectOfType<PlayerGun>();
        if (pRoot != null)
        {
            player = pRoot.transform;
            if (Vector3.Distance(player.position, transform.position) <= distanceToPickup)
            {
                if (!inZone)
                {
                    AudioManager.Play(soundEffect, 1, 1, false);
                    FindObjectOfType<PlayerStats>().health += 20;
                    Destroy(gameObject);
                }
                inZone = true;
            }
            else
            {
                inZone = false;
            }
        }

        // Find enemies in vicinity
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        bool foundEnemies = false;
        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) <= distanceToPickup)
            {
                foundEnemies = true;
                if (!enemyInZone)
                {
                    enemyInZone = true;
                    enemy.health += 20;
                    Destroy(gameObject);
                }
            }
        }

        if(!foundEnemies && enemyInZone)
        {
            enemyInZone = false;
        }
    }
}
