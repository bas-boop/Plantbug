using System.Collections;
using TMPro;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour
{
    private TMP_Text m_TextMeshPro;
    private Color startColor = new Color(1,1,1,0);
    private Color targetColor = new Color(1, 1, 1, 1);
    [SerializeField] private float duration = 2f;
    [SerializeField] private float delay;

    private bool isFadedIn = false;
    private void Start()
    {
        isFadedIn = false;
        m_TextMeshPro = GetComponent<TMP_Text>();
        m_TextMeshPro.color = startColor;
        LeanTween.delayedCall(delay, CallFadeInOrOut);
    }

    private void CallFadeInOrOut()
    {
        if (!isFadedIn)
            StartCoroutine(FadeIn());

        if (isFadedIn)
            StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            m_TextMeshPro.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        m_TextMeshPro.color = targetColor;

        isFadedIn = true;
        LeanTween.delayedCall(4f, CallFadeInOrOut);

    }

    public IEnumerator FadeOut()
    {
        float elapsed = 0f;    

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            m_TextMeshPro.color = Color.Lerp(targetColor, startColor, t);
            yield return null;
        }

        m_TextMeshPro.color = startColor;
        isFadedIn = false;

        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
