using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float despawnAfter = 2f;
    private float awakeAt = 0;
    [HideInInspector]
    public float damage;

    [Header("Impacts")]
    public GameObject metalImpact;
    public GameObject humanImpact;

    private void Start()
    {
        awakeAt = Time.time;
    }

    private void FixedUpdate()
    {
        // Move forward (wow, amazing programming skill)
        Vector3 newPredictedPos = (transform.forward * speed) + transform.position;

        if(Time.time - awakeAt > despawnAfter)
        {
            Destroy(gameObject);
        }

        // Raycast Hit Detection
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, speed))
        {
            Vector3 normal = hit.normal;

            // Check what the bullet hit
            GameObject impact;
            if (hit.collider.gameObject.tag == "Enemy")
            {
                // Sound
                AudioManager.Play("BulletHit", 1, Random.Range(0.7f, 1f), false);

                impact = humanImpact;

                // Was it a headshot
                float toTake = damage;
                if (hit.collider.gameObject.name == "Headshot")
                    toTake = 100;

                // Go up through the enemy hierachy until it's reached the root and add the damage
                bool foundRoot = false;
                Transform currentPos = hit.collider.transform;
                while(!foundRoot)
                {
                    currentPos = currentPos.parent;
                    if(currentPos == null)
                    {
                        // There's no root (probably a ragdoll)
                        break;
                    }
                    if(currentPos.GetComponent<Enemy>())
                    {
                        foundRoot = true;
                        currentPos.GetComponent<Enemy>().TakeDamage(toTake, transform.position, transform.forward);
                    }
                }
            }
            else
            {
                impact = metalImpact;
            }

            GameObject impactSpawned = Instantiate(impact, hit.point + (normal/20), Quaternion.LookRotation(normal));
            impactSpawned.transform.parent = hit.collider.transform;
            Destroy(gameObject);
        }

        transform.position = Vector3.Lerp(transform.position, newPredictedPos, Time.deltaTime);
    }
}
