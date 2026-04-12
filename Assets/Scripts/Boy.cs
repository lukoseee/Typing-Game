using UnityEngine;
using System.Collections;

public class Boy : MonoBehaviour
{   
    [SerializeField] private float moveSpeed = 5f;
    private Animator animator;
    [SerializeField] private Vector3 initialPosition;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private int targetCandleIndex;
    [SerializeField] private CandleController candleController;
    [SerializeField] private GameController gameController;
    
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

        targetPosition = new Vector3(candlePos.x, transform.position.y, transform.position.z);

        isMoving = true;
        animator.SetBool("isWalking", true);
    }

    private void HandleMovement()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.05f)
        {
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
