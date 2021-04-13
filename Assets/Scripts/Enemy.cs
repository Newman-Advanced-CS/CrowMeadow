using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string Name;
    public string[] names;

    [Range(0, 100)]
    public float health = 100;
    public GameObject ragdoll;

    private void Start()
    {
        // Assign the enemy a random name
        if (Name == "")
            Name = names[Random.Range(0, names.Length)];
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

            // Destroy the enemy
            Destroy(gameObject);
        }
    }

    //TODO: Xavier's enemy AI
}
