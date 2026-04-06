using UnityEngine;
using UnityEngine.UI;

public class GraceDisplay : MonoBehaviour
{
    [SerializeField] private Text graceText;

    private void OnEnable()
    {   
        if(GraceManager.Instance != null)
        {
            GraceManager.Instance.OnGraceChanged += UpdateGraceDisplay;
            UpdateGraceDisplay(GraceManager.Instance.getGraceAmount());
        }
    }

    private void OnDisable()
    {
        if (GraceManager.Instance != null)
            GraceManager.Instance.OnGraceChanged -= UpdateGraceDisplay;
    }

    private void UpdateGraceDisplay(int graceAmount)
    {
        graceText.text = graceAmount.ToString();
    }
}
