using UnityEngine;

public class GunManager : MonoBehaviour
{
    public GameObject Gun1;
    private GameObject Gun1Slot;
    public GameObject Gun2;
    private GameObject Gun2Slot;
    public Transform GunParent;

    [Header("Gun UI")]
    public UnityEngine.UI.Image crosshair;
    public Sprite crosshairHip;
    public Sprite crosshairAim;

    private void Start()
    {
        Calibrate();
    }

    public void Calibrate()
    {
        if(Gun1Slot != null) { Destroy(Gun1Slot); }
        if(Gun2Slot != null) { Destroy(Gun2Slot); }

        // Instantiate the right guns
        Gun1Slot = Instantiate(Gun1, GunParent);
        Gun2Slot = Instantiate(Gun2, GunParent);

        EquipGun(true);
    }

    private void Update()
    {
        // Equip the right gun
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipGun(true);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipGun(false);
        }
    }

    public void EquipGun(bool first)
    {
        // Show the right gun
        switch (first)
        {
            case true:
                Gun2Slot.GetComponent<PlayerGun>().isReloading = false;
                Gun2Slot.GetComponent<PlayerGun>().anim.transform.localEulerAngles = new Vector3(0,90,0);
                Gun1Slot.SetActive(true);
                Gun2Slot.SetActive(false);

                // Disable Sniper Overlay
                if (Gun2Slot.GetComponent<SniperRifle>() != null)
                {
                    FindObjectOfType<UIManager>().SniperOverlay.SetActive(false);
                }
                break;
            case false:
                Gun1Slot.GetComponent<PlayerGun>().isReloading = false;
                Gun1Slot.GetComponent<PlayerGun>().anim.transform.localEulerAngles = new Vector3(0, 90, 0);
                Gun1Slot.SetActive(false);
                Gun2Slot.SetActive(true);

                // Disable Sniper Overlay
                if(Gun1Slot.GetComponent<SniperRifle>() != null)
                {
                    FindObjectOfType<UIManager>().SniperOverlay.SetActive(false);
                }
                break;
        }
    }

    public void SightsStatus(bool status)
    {
        Gun1Slot.GetComponent<PlayerGun>().canAimSights = status;
        Gun2Slot.GetComponent<PlayerGun>().canAimSights = status;
    }
}
