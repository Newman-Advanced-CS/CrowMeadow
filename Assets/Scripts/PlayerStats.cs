using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Txt Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI gunAmmoText;

    [Header("Values")]
    [Range(0, 100)]
    public float health = 100f;
    public int ammoInClip = 32;
    public int ammoInTotal = 256;
    public string gunEquiped = "default";

    private void Update()
    {
        // Set all the text elements to their respected values
        healthText.text = health.ToString() + "%";
        gunNameText.text = gunEquiped;
        gunAmmoText.text = ammoInClip.ToString() + "/" + ammoInTotal.ToString();

        if(ammoInClip <= 5 && ammoInClip > 0)
        {
            gunAmmoText.color = Color.yellow;
        }
        else if(ammoInClip <= 0)
        {
            gunAmmoText.color = Color.red;
        }
        else
        {
            gunAmmoText.color = Color.white;
        }
    }
}
