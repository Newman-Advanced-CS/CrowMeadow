using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialTriggerData
{
    [TextArea(2, 5)]
    public string dialogue;
    public Collider trigger;
}

public class TutorialTrigger : MonoBehaviour
{
    public string dialogue;
    public UIManager ui;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<GunManager>() != null)
        {
            ui.Dialouge(dialogue);
            Destroy(gameObject);
        }
    }
}

public class Tutorial : MonoBehaviour
{
    UIManager ui;
    bool started = false;
    bool ended = false;

    public TutorialTriggerData[] triggers;

    void Update()
    {
        if(FindObjectOfType<WeaponScreen>() == null && !started)
        {
            // Get UI Manager and start dialogue
            ui = FindObjectOfType<UIManager>();
            started = true;

            ui.Dialouge("Welcome to basic training! Get through this and you'll be cleared for field duty!");

            // Set up triggers
            foreach (TutorialTriggerData triggerData in triggers)
            {
                TutorialTrigger newTrigger = triggerData.trigger.gameObject.AddComponent<TutorialTrigger>();
                newTrigger.ui = ui;
                newTrigger.dialogue = triggerData.dialogue;
            }
        }

        // Check for win state
        if(!ended)
        {
            foreach (ControlPoint point in FindObjectsOfType<ControlPoint>())
            {
                if (point.ownership != ControlPoint.Ownership.BLUE)
                    return;
            }
            ui.Dialouge("Congrats recruit! You have now been cleared for field duty!");
            StartCoroutine(exit());
            ended = true;
        }
    }

    IEnumerator exit()
    {
        yield return new WaitForSeconds(3);
        ui.StartFade(Color.black, 2, delegate
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });
    }
}
