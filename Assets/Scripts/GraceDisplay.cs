using UnityEngine;
using UnityEngine.UI;

//displays current grace amount in UI and updates when grace changes
public class GraceDisplay : MonoBehaviour
{
    [SerializeField] private Text graceText;

    private void OnEnable()
    {   
        if(GraceManager.Instance != null)
        {   
            //subscribe to grace change event and initialize display
            GraceManager.Instance.OnGraceChanged += UpdateGraceDisplay;
            UpdateGraceDisplay(GraceManager.Instance.getGraceAmount());
        }
    }

    private void OnDisable()
    {   //unsubscribe from event 
        if (GraceManager.Instance != null)
            GraceManager.Instance.OnGraceChanged -= UpdateGraceDisplay;
    }

    private void UpdateGraceDisplay(int graceAmount)
    {   
        graceText.text = graceAmount.ToString();
    }
}
