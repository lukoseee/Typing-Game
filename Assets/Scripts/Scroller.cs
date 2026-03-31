using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{   
    [SerializeField] private RawImage img;
    [SerializeField] private float x, y;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        img.uvRect = new Rect(img.uvRect.position + new Vector2(x,y) * Time.deltaTime, img.uvRect.size);
    }
}
