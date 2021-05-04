using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI importantText;
    public GameObject SniperOverlay;
    public TextMeshProUGUI zoomLevelText;
    public GameObject mapScreen;
    public GameObject dialouge;
    public GameObject fadeScreen;

    private void Start()
    {
        SniperOverlay.SetActive(false);
        dialouge.SetActive(false);
        fadeScreen.SetActive(false);
    }

    private void Update()
    {
        mapScreen.SetActive(Input.GetKey(KeyCode.M));
    }

    public void SetZoomLevelDisplay(int zoomLevel)
    {
        zoomLevelText.text = "Zoom Level: " + (zoomLevel + 1).ToString();
    }

    public void Announce(string toAnnounce)
    {
        StartCoroutine(AnnounceTimer(toAnnounce));
    }

    IEnumerator AnnounceTimer(string toAnnounce)
    {
        importantText.gameObject.SetActive(true);
        importantText.text = toAnnounce;
        yield return new WaitForSeconds(1.5f);
        importantText.gameObject.SetActive(false);
    }

    public void Dialouge(string toSay)
    {
        // Play the sound
        AudioManager.Play("Com", 1, 1, false);

        StartCoroutine(SpeakingDialogue(toSay));
    }

    IEnumerator SpeakingDialogue(string toSay)
    {
        yield return new WaitForSeconds(0.5f);
        dialouge.SetActive(true);

        Animation anim = dialouge.GetComponent<Animation>();
        TextMeshProUGUI text = dialouge.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "";

        anim.Play("DialogueStart");
        yield return new WaitForSeconds(0.2f);

        char[] characters = toSay.ToCharArray();
        foreach (char character in characters)
        {
            text.text += character;
            yield return new WaitForSeconds(0.02f);
            AudioManager.Play("VoiceBlip", 1f, Random.Range(0.5f, 1f), false);
        }

        yield return new WaitForSeconds(2f);
        text.text = "";
        anim.Play("DialogueEnd");

        yield return new WaitForSeconds(0.2f);
        dialouge.SetActive(false);
    }

    // Fade out to a certain color, and call a custom delegate void at the end
    [HideInInspector]
    public delegate void ArgumentVoid();
    public void StartFade(Color screenColor, float speed, ArgumentVoid toRun)
    {
        fadeScreen.SetActive(true);
        screenColor.a = 0;

        StartCoroutine(FadeFrame(screenColor, speed, toRun));
    }

    IEnumerator FadeFrame(Color screenColor, float speed, ArgumentVoid toRun)
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(screenColor);
        fadeScreen.GetComponent<Image>().color = screenColor;

        if (screenColor.a >= 1f)
        {
            toRun();
        }
        else
        {
            screenColor.a += Time.deltaTime;
            StartCoroutine(FadeFrame(screenColor, speed, toRun));
        }
    }
}
