using UnityEngine;

//manages camera movement to follow the player
public class CameraFollow : MonoBehaviour
{   
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed = 3f;
    private Vector3 initialPosition;
    private bool isRewinding = false;
    private bool isFrozen = false;

    void Awake()
    {
        initialPosition = transform.position;
    }

    //late update to ensure camera moves after player has moved in update
    void LateUpdate(){
        
        //sling camera back to start
        if(isRewinding){
            transform.position = Vector3.Lerp(
                transform.position,
                initialPosition,
                Time.deltaTime * speed         
            );
            if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
            {
                transform.position = initialPosition;
                isRewinding = false;
            }
        } else if(!isFrozen){

            transform.position = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

        }
    }

    public void StartRewind()
    {   
        isRewinding = true;
        isFrozen = true;
    }
    
    //on reset or level complete, snap camera back to initial position (even if in midde of rewind)
    public void ResetPosition()
    {   
        isRewinding = false;
        isFrozen = false;
        transform.position = initialPosition;
    }
}
