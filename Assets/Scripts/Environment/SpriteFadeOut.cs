using System.Collections;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteFadeOut : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 2f;
        
        private SpriteRenderer _spriteRenderer;

        private void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();

        public void StartFade()
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutCoroutine());
        }

        private IEnumerator FadeOutCoroutine()
        {
            float alpha = 1f;
            float fadeSpeed = 1f / fadeDuration;

            while (alpha > 0f)
            {
                alpha -= fadeSpeed * Time.deltaTime;
                alpha = Mathf.Clamp01(alpha);

                Color color = _spriteRenderer.color;
                color.a = alpha;
                _spriteRenderer.color = color;

                yield return null;
            }
        }
    }
}