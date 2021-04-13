using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject disclaimerScreen;

    private void Start()
    {
        LeaveDisclaimer();
        AudioManager.Play("MainMenu", 1, 1, true);
    }

    public void GoToDisclaimer()
    {
        disclaimerScreen.SetActive(true);
    }

    public void LeaveDisclaimer()
    {
        disclaimerScreen.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }
}
