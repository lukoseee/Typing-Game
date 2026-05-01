using UnityEngine;

//candle prefab
public class Candle : MonoBehaviour
{   
    private Animator animator = null;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
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
