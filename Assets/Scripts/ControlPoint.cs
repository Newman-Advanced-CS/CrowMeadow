using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public enum Ownership
    {
        BLUE,
        RED,
        UNCAPTURED
    }
    public Ownership ownership;
    public string Name;
    public float captureDistance = 20f;

    // Capure variables
    public float timeNeededToCapture = 10f;
    private float progress;
    [HideInInspector]
    public int influence = 0;

    [Header("Components")]
    public Renderer flagRenderer;
    public Renderer mapIcon;
    public GameObject enemy;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, captureDistance);
    }

    private void Start()
    {
        progress = timeNeededToCapture;

        if(ownership == Ownership.RED)
        {
            for(int i = 0; i < 5; i++)
            {
                Instantiate(enemy, transform.position, Quaternion.identity);
            }
        }
    }

    public void Update()
    {
        // Update the color depending on the ownership
        switch(ownership)
        {
            case Ownership.BLUE:
                mapIcon.material.color = Color.blue;
                flagRenderer.material.color = Color.blue;
                break;
            case Ownership.RED:
                mapIcon.material.color = Color.red;
                flagRenderer.material.color = Color.red;
                break;
            case Ownership.UNCAPTURED:
                mapIcon.material.color = Color.white;
                flagRenderer.material.color = Color.gray;
                break;
        }

        // CALCULATE INFLUENCE
        influence = 0;

        // Check if the player is within capturing distance
        PlayerGun PlayerRoot = FindObjectOfType<PlayerGun>();
        if (PlayerRoot != null)
        {
            Transform Player = PlayerRoot.transform;
            if (Vector3.Distance(Player.position, transform.position) <= captureDistance)
            {
                influence++;
            }
        }

        // Check if enemies are within capturing distance
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            if(Vector3.Distance(enemy.transform.position, transform.position) <= captureDistance)
            {
                influence--;
            }
        }

        // Calucalate progress based on influence
        if(influence > 0)
        {
            // Move towards blue
            if(ownership != Ownership.BLUE)
            {
                progress -= Time.deltaTime;
                if(progress < 0)
                {
                    ownership = Ownership.BLUE;
                    FindObjectOfType<UIManager>().Announce("BLUE has captured: " + Name);
                }
            }
            else
            {
                progress += Time.deltaTime;
                if (progress > timeNeededToCapture)
                {
                    progress = timeNeededToCapture;
                }
            }
        }

        if (influence < 0)
        {
            // Move towards blue
            if (ownership != Ownership.RED)
            {
                progress -= Time.deltaTime;
                if (progress < 0)
                {
                    ownership = Ownership.RED;
                    FindObjectOfType<UIManager>().Announce("RED has captured: " + Name);
                }
            }
            else
            {
                progress += Time.deltaTime;
                if (progress > timeNeededToCapture)
                {
                    progress = timeNeededToCapture;
                }
            }
        }

        // Calcalute Y based on progress
        float y = 7.5f * (progress / timeNeededToCapture) + 1.5f;
        flagRenderer.transform.localPosition = new Vector3(flagRenderer.transform.localPosition.x, y, flagRenderer.transform.localPosition.z);
    }
}
