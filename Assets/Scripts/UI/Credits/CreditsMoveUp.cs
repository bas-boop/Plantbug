using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class CreditsMoveUp : MonoBehaviour
{
    [Header("Anim Settings")]
    [SerializeField] private float endPositionY;
    [SerializeField] private float moveUpDuration;
    [SerializeField] private LeanTweenType type;

    [Header("Glitch Settings")]
    [SerializeField] private float glitchAmount;
    [SerializeField] private float glitchDuration;
    [SerializeField] private LeanTweenType glitchType = LeanTweenType.easeShake;
    
    private Vector3 orgPos;

    private float timer = 1;

    private bool isGlitching = false;
    private void Start()
    {
        LeanTween.delayedCall(1f, MoveUp);
    }

    private void Update()
    {
        if (!isGlitching)
            return;

        timer += Time.deltaTime;
    }

    private void MoveUp()
    {
        LeanTween.moveLocalY(gameObject, endPositionY, moveUpDuration).setEase(type).setOnComplete(() =>
        {
            orgPos = transform.localPosition;
        }).setOnComplete(GlitchCredits);
    }

    private void GlitchCredits()
    {
        timer = 0;
        isGlitching = true;

        LeanTween.value(gameObject, 0, glitchAmount, glitchDuration).setOnUpdate((float val) =>
        {
            float x = Random.Range(0, glitchAmount) * timer * 2;
            float y = Random.Range(0, glitchAmount) * timer;

            transform.localPosition = orgPos + new Vector3(x, y, 0f);
        })
            .setOnComplete(() =>
        {
            transform.localPosition = orgPos;
        });

    }
}
