using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public ControlPoint.Ownership winner = ControlPoint.Ownership.UNCAPTURED;
    ControlPoint[] points;

    bool started = false;
    bool ended = false;
    UIManager ui;

    private void Start()
    {
        points = FindObjectsOfType<ControlPoint>();
    }

    void Update()
    {
        // Start the game off with Stefano
        if(FindObjectOfType<WeaponScreen>() == null && !started)
        {
            ui = FindObjectOfType<UIManager>();
            started = true;
            ui.Dialouge("Mission is go! Get those points!");
        }

        ControlPoint.Ownership winner = checkWinner();
        if (winner != ControlPoint.Ownership.UNCAPTURED && !ended)
        {
            // Specific dialogue
            switch (winner)
            {
                case ControlPoint.Ownership.BLUE:
                    ui.Dialouge("All points are under our control! Good job!");
                    StartCoroutine(fadeToMainMenu());
                    break;
                case ControlPoint.Ownership.RED:
                    ui.Dialouge("We've lost all control! Retreat!");
                    StartCoroutine(fadeToMainMenu());
                    break;
            }
            ended = true;
        }
    }

    ControlPoint.Ownership checkWinner()
    {
        // Check control point status
        winner = points[0].ownership;
        foreach (ControlPoint point in points)
        {
            if (point.ownership != winner)
            {
                return ControlPoint.Ownership.UNCAPTURED;
            }
        }

        return winner;
    }

    IEnumerator fadeToMainMenu()
    {
        yield return new WaitForSeconds(1.5f);
        ui.StartFade(Color.black, 1, delegate {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	    });
    }
}
