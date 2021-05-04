using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject disclaimerScreen;
    public GameObject mapScreen;
    public GameObject mapDetailScreen;
    public Transform mapListContent;
    public TextMeshProUGUI mapNameText;
    public TextMeshProUGUI mapDescText;
    public Image mapImage;
    public Button goButton;

    [Header("Prefabs")]
    public GameObject mapButton;

    [Header("Other")]
    public MapListing[] maps;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        mapDetailScreen.SetActive(false);
        LeaveDisclaimer();
        AudioManager.Play("MainMenu", 1, 1, true);

        // Populate map list
        foreach (MapListing mapListing in maps)
        {
            GameObject newListing = Instantiate(mapButton, mapListContent);
            newListing.GetComponentInChildren<TextMeshProUGUI>().text = mapListing.MapName;
            newListing.GetComponentInChildren<Button>().onClick.AddListener(delegate { LoadLevelData(mapListing); });
        }

        BackToMainMenu();
    }

    public void LoadLevelData(MapListing map)
    {
        mapDetailScreen.SetActive(true);
        mapNameText.text = map.MapName;
        mapDescText.text = map.MapDescription;
        mapImage.sprite = map.Image;

        goButton.onClick.AddListener(delegate { SceneManager.LoadScene(map.sceneIndex); } );
    }

    public void GoToDisclaimer()
    {
        disclaimerScreen.SetActive(true);
    }

    public void LeaveDisclaimer()
    {
        disclaimerScreen.SetActive(false);
    }

    public void BackToMainMenu()
    {
        mapScreen.SetActive(false);
    }

    public void Play()
    {
        mapScreen.SetActive(true);
    }
}
