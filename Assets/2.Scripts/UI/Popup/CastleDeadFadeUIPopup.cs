using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastleDeadFadeUIPopup : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private float fadeInDuration = 2f;

    private Image fadeImage;

    public void Init()
    {
        fadeImage = GetComponent<Image>();
        gameObject.SetActive(false);
        FindAnyObjectByType<Castle>().OnDead += Fade;
    }

    private void Fade()
    {
        gameObject.SetActive(true);
        StartCoroutine(CoFadeIn());
    }

    private IEnumerator CoFadeIn()
    {
        float elapsedTime = Time.deltaTime;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (elapsedTime < fadeInDuration)
        {
            float ratio = elapsedTime / fadeInDuration; // Calculate ratio

            // Simple linear interpolation
            float alpha = Mathf.Lerp(1f, 0f, Mathf.SmoothStep(0f, 1f, ratio));
            color.a = alpha;
            fadeImage.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        gameObject.SetActive(false);
    }
}
