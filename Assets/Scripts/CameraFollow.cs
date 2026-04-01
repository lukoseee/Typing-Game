using UnityEngine;

public class CameraFollow : MonoBehaviour
{   
    public Transform target;
    public Vector3 offset;
    private Vector3 initialPosition;


    void Awake()
    {
        initialPosition = transform.position;
    }

    void LateUpdate(){
        transform.position = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

    }
    
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
