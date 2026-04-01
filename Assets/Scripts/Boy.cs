using UnityEngine;
using System.Collections;

public class Boy : MonoBehaviour
{   public float moveSpeed = 5f;
    private Animator animator;
    public Vector3 initialPosition;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private int targetCandleIndex;
    public CandleController candleController;
    public GameController gameController;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
    }

    public void MoveToCandle(int candleIndex)
    {
        targetCandleIndex = candleIndex;

        Vector3 candlePos = candleController.GetCandlePosition(candleIndex);

        // Only move on X axis
        targetPosition = new Vector3(candlePos.x, transform.position.y, transform.position.z);

        isMoving = true;
        animator.SetBool("isWalking", true);
    }

    private void HandleMovement()
    {
        if (!isMoving) return;

        // Move toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if reached
        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.05f)
        {
            // Stop walking and play Light animation
            isMoving = false;
            animator.SetBool("isWalking", false);
            candleController.lightNextCandle(targetCandleIndex);
        }
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        isMoving = false;
        animator.SetBool("isWalking", false);
        animator.ResetTrigger("Light");
    }

}
