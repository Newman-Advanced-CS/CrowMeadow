using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    public float rotationSpeed = 5;
    private float currentRotation = 0;
    void Update()
    {
        currentRotation += Time.deltaTime * rotationSpeed;
        if (currentRotation > 360)
            currentRotation -= 360;

        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
    }
}
