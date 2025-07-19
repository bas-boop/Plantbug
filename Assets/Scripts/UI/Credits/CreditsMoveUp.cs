using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class CreditsMoveUp : MonoBehaviour
{
    [Header("Anim Settings")]
    [SerializeField] private float endPositionY;
    [SerializeField] private float moveUpDuration;

    [Header("Glitch Settings")]
    [SerializeField] private float glitchAmount;
    [SerializeField] private float glitchDuration;
    
    private Vector3 orgPos;

    private float timer = 1;

    private bool isGlitching = false;
    private bool glitchIsReset = false;
    private void Start()
    {
        LeanTween.delayedCall(1f, MoveUp);
    }

    private void Update()
    {
        if (!isGlitching)
            return;

        timer += Time.deltaTime;


        if (timer >= .2f && !glitchIsReset)
        {
            timer = 0;
            glitchIsReset = true;
        }
    }

    private void MoveUp()
    {

        LeanTween.moveLocalY(gameObject, endPositionY/2, moveUpDuration).setOnComplete(() =>
        {
            orgPos = transform.localPosition;
            MiniGlitchCredits();
        });
    }

    private void MiniGlitchCredits()
    {
        LeanTween.value(gameObject, 0, glitchAmount, .05f).setOnUpdate((float val) =>
        {
            float x = Random.Range(0, glitchAmount);
            float y = Random.Range(0, glitchAmount);

            transform.localPosition = orgPos + new Vector3(x, y, 0f);
        })
            .setOnComplete(() =>
            {
                transform.localPosition = orgPos;
                MoveUp2();
            });
    }

    private void MoveUp2()
    {
        LeanTween.moveLocalY(gameObject, endPositionY, moveUpDuration).setOnComplete(() =>
        {
            orgPos = transform.localPosition;
            GlitchCredits();
        });
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
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInElastic().setDestroyOnComplete(true);
            transform.localPosition = orgPos;
        });

    }
}
