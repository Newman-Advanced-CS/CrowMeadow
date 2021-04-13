using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "ScriptableObjects/Gun")]
public class Gun : ScriptableObject
{
    [Header("Gun Basics")]
    public string GunName = "New Gun";
    public float shootingInterval = 0.2f;
    public bool constantFire = true;
    public string shotClip;

    [Header("Accuracy")]
    [Range(0, 100)]
    public float accuracyAtHip = 80f;
    [Range(0, 100)]
    public float accuracyAtSights = 80f;

    [Header("Gun Values")]
    public int ammoPerMagazine = 32;
    public float baseRecoilAmount = 2f;
    public float damage = 15f;
}
