using TMPro;
using UnityEngine;
using System.Collections;

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

    bool isDead = false;
    UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

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

        // Has the player died
        if(health <= 0 && !isDead)
        {
            isDead = true;
            UIManager.ArgumentVoid deathFunc = delegate {
                FindObjectOfType<GunManager>().enabled = false;
                FindObjectOfType<PlayerGun>().enabled = false;

                // Respawn
                StartCoroutine(respawn());
            };

            uiManager.StartFade(new Color(1, 0, 0, 0), 1.5f, deathFunc);
        }
    }

    IEnumerator respawn()
    {
        yield return new WaitForSeconds(3f);
        FindObjectOfType<GunManager>().enabled = true;
        FindObjectOfType<PlayerGun>().enabled = true;
        uiManager.fadeScreen.SetActive(false);
        health = 100;

        // Find control point to respawn at
        foreach(ControlPoint point in FindObjectsOfType<ControlPoint>())
        {
            if(point.ownership == ControlPoint.Ownership.BLUE)
            {
                FindObjectOfType<GunManager>().transform.position = point.transform.position;
                yield break;
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
