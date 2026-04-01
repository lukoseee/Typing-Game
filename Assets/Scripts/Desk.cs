using UnityEngine;

public class Desk : MonoBehaviour
{   
    public Transform middle;
    public float baseMiddleWidth = 1f;

    public Transform rightLeg;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseMiddleWidth = middle.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public void ResizeDesk(int candleCount, int spacing){

        float width = (candleCount - 1) * spacing;

        // Scale middle (grows right only)
        Vector3 scale = middle.localScale;
        scale.x = width / baseMiddleWidth;
        middle.localScale = scale;

        // Left leg stays fixed
        // Right leg moves to end
        rightLeg.localPosition = new Vector3(middle.localPosition.x + width, rightLeg.localPosition.y, 0f);

    }
}
