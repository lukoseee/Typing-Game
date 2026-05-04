using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Powerup icon popups when used.
public class PowerupPopup : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    [SerializeField] private Sprite sacredPauseIcon;
    [SerializeField] private Sprite forgivingFlameIcon;

    [SerializeField] private float popDuration = 0.2f;
    [SerializeField] private float holdDuration = 0.4f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private float startScale = 0.5f;
    [SerializeField] private float peakScale = 1.2f;

    public static void Spawn(PowerupPopup prefab, Transform parent, string iconName)
    {
        PowerupPopup popup = Instantiate(prefab, parent);
        popup.iconImage.sprite = popup.GetIcon(iconName);
        popup.StartCoroutine(popup.Animate());
    }

    private Sprite GetIcon(string iconName)
    {
        switch (iconName)
        {
            case "SacredPause": return sacredPauseIcon;
            case "ForgivingFlame":   return forgivingFlameIcon;
            default:
                Debug.LogWarning($"PowerupPopup: unknown icon '{iconName}'");
                return null;
        }
    }

    private void SetAlpha(float a)
    {
        Color c = iconImage.color;
        c.a = a;
        iconImage.color = c;
    }

    private IEnumerator Animate()
    {
        SetAlpha(0f);
        transform.localScale = Vector3.one * startScale;

        // pop in 
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            float t = elapsed / popDuration;
            t = 1f - Mathf.Pow(1f - t, 3f); // ease-out cubic
            SetAlpha(t);
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, peakScale, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        SetAlpha(1f);
        transform.localScale = Vector3.one;

        yield return new WaitForSeconds(holdDuration);

        // fade out 
        yield return FadeUtil.Lerp(1f, 0f, fadeDuration, SetAlpha);

        Destroy(gameObject);
    }
}