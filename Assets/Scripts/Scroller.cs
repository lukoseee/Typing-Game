using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{   
    [SerializeField] private RawImage img;
    [SerializeField] private float parallaxStrength = 0.02f;
    private Camera mainCamera;
    private float lastCameraX;

    void Start()
    {
        mainCamera = Camera.main;
        lastCameraX = mainCamera.transform.position.x;
    }

    void Update()
    {
        float cameraDelta = mainCamera.transform.position.x - lastCameraX;
        img.uvRect = new Rect(img.uvRect.position + new Vector2(cameraDelta * parallaxStrength, 0), img.uvRect.size);
        lastCameraX = mainCamera.transform.position.x;
    }
}
