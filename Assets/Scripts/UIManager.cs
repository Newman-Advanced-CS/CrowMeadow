using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI importantText;
    public GameObject SniperOverlay;
    public TextMeshProUGUI zoomLevelText;

    private void Start()
    {
        SniperOverlay.SetActive(false);
    }

    public void SetZoomLevelDisplay(int zoomLevel)
    {
        zoomLevelText.text = "Zoom Level: " + (zoomLevel+1).ToString();
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
}
