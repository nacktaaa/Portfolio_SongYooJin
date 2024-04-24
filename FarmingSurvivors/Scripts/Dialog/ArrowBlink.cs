using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowBlink : MonoBehaviour
{
    [SerializeField]
    float fadeTime;
    Image fadeImage;

    private void Awake() {
        fadeImage = GetComponent<Image>();
    }

    private void OnEnable() {
        StartCoroutine("FadeInOut");
    }

    private void OnDisable() {
        StopCoroutine("FadeInOut");
    }

    IEnumerator FadeInOut()
    {
        while(true)
        {
            yield return StartCoroutine(Fade(1,0));
            yield return StartCoroutine(Fade(0,1));
        }
    }

    IEnumerator Fade(float start, float end)
    {
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.unscaledDeltaTime;
            percent = current / fadeTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadeImage.color = color;

            yield return null;
        }
    }

}
