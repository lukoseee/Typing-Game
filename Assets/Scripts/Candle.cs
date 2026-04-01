using UnityEngine;

public class Candle : MonoBehaviour
{   
    private Animator animator = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Light()
    {
        animator.SetTrigger("Light");
    }

    public void ResetCandle()
    {
        animator.Play("Unlit");
    }

    public void BlowOut()
    {
        animator.SetTrigger("fail");
    }
}
