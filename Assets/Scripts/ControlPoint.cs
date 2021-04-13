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
    public float captureDistance = 20f;

    public float timeNeededToCapture = 10f;
    private float timeTaken;

    private Vector3 originalFlagPos;
    private bool flagAtBottom = false;

    [Header("Components")]
    public Renderer flagRenderer;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, captureDistance);
    }

    private void Start()
    {
        originalFlagPos = flagRenderer.transform.position;
        timeTaken = timeNeededToCapture;
    }

    public void Update()
    {
        // Update the color depending on the ownership
        switch(ownership)
        {
            case Ownership.BLUE:
                flagRenderer.material.color = Color.blue;
                break;
            case Ownership.RED:
                flagRenderer.material.color = Color.red;
                break;
            case Ownership.UNCAPTURED:
                flagRenderer.material.color = Color.gray;
                break;
        }

        if(ownership != Ownership.BLUE)
        {
            // Check if the player is within capturing distance
            PlayerGun PlayerRoot = FindObjectOfType<PlayerGun>();
            if(PlayerRoot != null)
            {
                Transform Player = PlayerRoot.transform;
                if (Vector3.Distance(Player.position, transform.position) <= captureDistance)
                {
                    timeTaken -= Time.deltaTime;

                    // Calculate the speed it needs to move at
                    float speedToMoveAt = 7.5f / (timeNeededToCapture);
                    Debug.Log(speedToMoveAt);

                    // ...and move it!
                    flagRenderer.transform.Translate(Vector3.down * speedToMoveAt * Time.deltaTime);

                    if (timeTaken <= 0)
                    {
                        ownership = Ownership.BLUE;
                        timeTaken = timeNeededToCapture;
                        flagAtBottom = true;
                    }
                }
            }
        }

        // Move the flag up if neccesary
        if(flagAtBottom)
        {
            flagRenderer.transform.position = Vector3.Lerp(flagRenderer.transform.position, originalFlagPos, Time.deltaTime * (timeNeededToCapture/2));

            if(Mathf.Round(flagRenderer.transform.position.y) == Mathf.Round(originalFlagPos.y))
            {
                flagAtBottom = false;
                flagRenderer.transform.position = originalFlagPos;
            }
        }
    }
}
