using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public string Name;
    public string[] names;

    [Range(0, 100)]
    public float health = 100;
    public GameObject ragdoll;
    public GameObject gun;
    public Transform leftHand;
    private Animator anim;
    public GameObject ammocrate;
    public GameObject healthcrate;

    // AI 
    private NavMeshAgent agent;
    private Transform player;

    private Vector3 randomDefencePoint;

    public enum States
    {
        DEFENDING_POINT,
        ATTACKING,
        RETREATING,
        ATTACKING_POINT
    }
    private States state = States.DEFENDING_POINT;

    private void Start()
    {
        // Get it's components
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Assign the enemy a random name
        if (Name == "")
            Name = names[Random.Range(0, names.Length)];

        // Calculate random point for defense
        randomDefencePoint = Random.insideUnitSphere * (FindObjectOfType<ControlPoint>().captureDistance / 2);
    }

    // Self-explanatory function
    public void TakeDamage(float damage, Vector3 impact, Vector3 impactDirection)
    {
        health -= damage;
        if(health <= 0)
        {
            // Disable all collision in the enemy
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach(Collider collider in colliders)
            {
                collider.enabled = false;
            }

            // Death
            GameObject corpse = Instantiate(ragdoll, transform.position, transform.rotation);
            ConfigureRagdollPosition(transform, corpse.transform);

            // Go through all the rigidbodies within the ragdoll to see which one was hit, and then add the impact force
            Rigidbody[] rigidbodies = corpse.GetComponentsInChildren<Rigidbody>();
            float closestDistance = Mathf.Infinity;
            Rigidbody closestBody = new Rigidbody();
            foreach(Rigidbody rb in rigidbodies)
            {
                // Calculate distance
                float calculatedDistance = Vector3.Distance(impact, rb.position);
                if(calculatedDistance <= closestDistance)
                {
                    closestDistance = calculatedDistance;
                    closestBody = rb;
                }
            }

            // Add the impact force
            closestBody.AddForce(impactDirection * 50, ForceMode.Impulse);
            Debug.Log("Adding impact force of: " + (impactDirection * 50).ToString());

            // Announce the kill
            FindObjectOfType<UIManager>().Announce("KILLED: " + Name);

            //Drop ammo box or Health kit
            int rand = Random.Range(0, 5);
            if (rand == 1)
            {
                Debug.Log("Spawning ammo crate at " + transform.position + ".");
                Instantiate(ammocrate, transform.position, Quaternion.identity);
            }
            else if (rand == 2)
            {
                Debug.Log("Spawning health crate at " + transform.position + ".");
                Instantiate(healthcrate, transform.position, Quaternion.identity);
            }

            StartCoroutine(respawn());
        }
    }

    IEnumerator respawn()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(4f);
        gameObject.SetActive(true);

        // Find control point to respawn at
        foreach (ControlPoint point in FindObjectsOfType<ControlPoint>())
        {
            if (point.ownership == ControlPoint.Ownership.RED)
            {
                transform.position = point.transform.position;
                yield break;
            }
        }
    }

    public virtual void Update()
    {
        // Update gun looking
        gun.transform.LookAt(leftHand);

        // Find the nearest control point
        float bestDistance = Mathf.Infinity;
        ControlPoint closestPoint = null;
        foreach(ControlPoint point in FindObjectsOfType<ControlPoint>())
        {
            if (point.ownership != ControlPoint.Ownership.RED && point.influence < 4)
            {
                float distance = Vector3.Distance(point.transform.position, transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closestPoint = point;
                }
            }
        }


        // Find the nearest health pickup
        float bestHealthDistance = Mathf.Infinity;
        HealthCrate closestHealth = null;
        foreach (HealthCrate healthCrate in FindObjectsOfType<HealthCrate>())
        {
                float distance = Vector3.Distance(healthCrate.transform.position, transform.position);
                if (distance < bestHealthDistance)
                {
                    bestHealthDistance = distance;
                    closestHealth = healthCrate;
                }
        }

        // Get if there's a player
        if (player == null)
        {
            if(FindObjectOfType<GunManager>() != null)
            {
                player = FindObjectOfType<GunManager>().transform;
            }
            else
            {
                return;
            } 
        }


        if(health > 20)
        {
            if (CheckIfInSights(player, transform.position))
            {
                state = States.ATTACKING;
            }
            else
            {
                if (bestDistance > closestPoint.captureDistance)
                {
                    state = States.ATTACKING_POINT;
                }
                else
                {
                    state = States.DEFENDING_POINT;
                }
            }
        }
        else
        {
            state = States.RETREATING;
        }

        // Act on state
        switch(state)
        {
            case States.ATTACKING:
                agent.isStopped = true;
                transform.LookAt(player);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 8.039f, 0);

                // Rotate Y-Axis of shooting pos
                Transform shootingPos = gun.GetComponent<GunObject>().shootingPos;
                shootingPos.LookAt(player);

                anim.SetBool("Shooting", true);

                // Shoot at the player
                GunObject gunProp = gun.GetComponent<GunObject>();
                if (gunProp.CanShoot())
                {
                    gunProp.Shoot();
                }
                if(gunProp.ammoInClip <= 0 && !gunProp.isReloading)
                {
                    gunProp.StartCoroutine(gunProp.Reload());
                    gunProp.ammoTotal += gunProp.gun.ammoPerMagazine;
                }
                break;
            case States.DEFENDING_POINT:
                // Defend
                agent.SetDestination(randomDefencePoint + closestPoint.transform.position);
                anim.SetBool("Shooting", agent.remainingDistance < 1);
                break;
            case States.RETREATING:
                // Go to nearest health crate
                agent.isStopped = false;
                if (closestHealth != null)
                {
                    agent.SetDestination(closestHealth.transform.position);
                    anim.SetBool("Shooting", false);
                }
                else
                {
                    Debug.Log("No health points found!");
                    state = States.DEFENDING_POINT;
                }
                break;
            case States.ATTACKING_POINT:
                if (closestPoint != null)
                {
                    agent.isStopped = false;
                    if (Vector3.Distance(transform.position, closestPoint.transform.position) >= 15)
                    {
                        agent.SetDestination(closestPoint.transform.position);
                        anim.SetBool("Shooting", false);
                    }
                    else
                    {
                        anim.SetBool("Shooting", false);
                        state = States.DEFENDING_POINT;
                    }
                }
                break;
        }
    }

    bool CheckIfInSights(Transform toCheck, Vector3 origin)
    {
        Vector3 offset = new Vector3(0, 1, 0);
        Vector3 direction = toCheck.position - origin;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast((origin + offset), direction, out hit, 50f, layerMask))
        {
            return (hit.transform == toCheck);
        }
        return false;
    }

    public void ConfigureRagdollPosition(Transform reference, Transform ragdollPart)
    {
        if (reference == gun.transform || reference.GetComponent<BoxCollider>() != null)
            return;

        ragdollPart.localPosition = reference.localPosition;
        ragdollPart.localRotation = reference.localRotation;

        for (int i = 0; i < ragdollPart.childCount; i++)
        {
            Transform ref_t = reference.GetChild(i);
            Transform rag_t = ragdollPart.GetChild(i);

            if (ref_t != null && rag_t != null)
                ConfigureRagdollPosition(ref_t, rag_t);
        }
    }

}
