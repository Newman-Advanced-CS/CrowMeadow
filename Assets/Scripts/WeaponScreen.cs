using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class WeaponPrefabData
{
    public Gun Data;
    public GameObject Gun;
}

public class WeaponScreen : MonoBehaviour
{
    public GameObject weaponPrefab;
    public Button ready;

    [Header("Weapons")]
    public WeaponPrefabData[] primaryGuns;
    public WeaponPrefabData[] secondaryGuns;

    [Header("Viewports")]
    public Transform primaryView;
    public Transform secondaryView;

    private GameObject gun1;
    private GameObject gun2;
    private GunManager pManager;
    
    private void Start()
    {
        // Show the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Deactive the player
        pManager = FindObjectOfType<GunManager>();
        pManager.gameObject.SetActive(false);

        // Fill the primary view
        foreach(WeaponPrefabData weapon in primaryGuns)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, primaryView);
            newWeapon.GetComponentInChildren<TextMeshProUGUI>().text = weapon.Data.GunName;
            newWeapon.GetComponent<Button>().onClick.AddListener(delegate { SetGunSlot(1, weapon.Gun, newWeapon.GetComponent<Button>()); });
        }

        // Fill the secondary view
        foreach (WeaponPrefabData weapon in secondaryGuns)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, secondaryView);
            newWeapon.GetComponentInChildren<TextMeshProUGUI>().text = weapon.Data.GunName;
            newWeapon.GetComponent<Button>().onClick.AddListener(delegate { SetGunSlot(2, weapon.Gun, newWeapon.GetComponent<Button>()); });
        }
    }

    private void Update()
    {
        ready.interactable = gun1 != null && gun2 != null;
    }

    public void SetGunSlot(int slot, GameObject gun, Button btn)
    {
        switch (slot)
        {
            case 1:
                gun1 = gun;

                // Update the colors of the buttons
                foreach (Button button in primaryView.GetComponentsInChildren<Button>())
                {
                    ColorBlock clrs = button.colors;
                    switch (button == btn)
                    {
                        case true:
                            clrs.normalColor = Color.yellow;
                            break;
                        case false:
                            clrs.normalColor = Color.white;
                            break;
                    }
                    button.colors = clrs;
                    button.GetComponent<Image>().color = clrs.normalColor;
                }

                break;
            case 2:
                gun2 = gun;

                // Update the colors of the buttons
                foreach (Button button in secondaryView.GetComponentsInChildren<Button>())
                {
                    ColorBlock clrs = button.colors;
                    switch (button == btn)
                    {
                        case true:
                            clrs.normalColor = Color.yellow;
                            break;
                        case false:
                            clrs.normalColor = Color.white;
                            break;
                    }
                    button.colors = clrs;
                    button.GetComponent<Image>().color = clrs.normalColor;
                }

                break;
            default:
                Debug.LogError("There's only two gun slots. Somethings wrong with the weapons screen.");
                break;
        }
    }

    public void Ready()
    {
        pManager.gameObject.SetActive(true);
        pManager.Gun1 = gun1;
        pManager.Gun2 = gun2;
        pManager.Calibrate();

        // Get rid of the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Destroy this screen
        Destroy(gameObject);
    }
}
