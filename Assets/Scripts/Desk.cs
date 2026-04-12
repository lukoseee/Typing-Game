using UnityEngine;

public class Desk : MonoBehaviour
{   
    [SerializeField] private Transform middle;
    [SerializeField] private float baseMiddleWidth = 1f;

    [SerializeField] private Transform rightLeg;
    
    void Start()
    {
        baseMiddleWidth = middle.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public void ResizeDesk(int candleCount, int spacing){

        float width = (candleCount - 1) * spacing;

        Vector3 scale = middle.localScale;
        scale.x = width / baseMiddleWidth;
        middle.localScale = scale;

        rightLeg.localPosition = new Vector3(middle.localPosition.x + width, rightLeg.localPosition.y, 0f);

    }
}
