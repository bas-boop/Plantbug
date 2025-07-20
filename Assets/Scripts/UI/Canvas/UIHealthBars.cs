using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using Gameplay;

namespace UI.Canvas
{
    public class UIHealthBars : MonoBehaviour
    {
        [Header("Health Bar Images (must be 3)")]
        [SerializeField] private List<Image> healthBars = new();

        [Header("Health Source")]
        [SerializeField] private Health healthSource;

        private int _totalHealth;
        private int _currentHealth;

        private int _healthPerBar;
        private List<int> _barValues = new();

        private void Awake()
        {
            if (healthSource == null)
                Debug.LogError("Health source is not assigned!");

            _totalHealth = healthSource.GetStartHealth();
            _currentHealth = _totalHealth;

            SetupBars();
        }

        private void Start()
        {
            StartFillingBars();
        }

        /// <summary>
        /// Starts filling the bars one by one based on current health.
        /// </summary>
        public void StartFillingBars()
        {
            StopAllCoroutines();
            StartCoroutine(FillBarsSequentially());
        }
        
        /// <summary>
        /// Updates the health bar scales based on the current health from the Health component.
        /// </summary>
        public void UpdateBars()
        {
            _currentHealth = Mathf.Clamp(healthSource.CurrentHealth, 0, _totalHealth);

            int healthRemaining = _currentHealth;

            for (int i = 0; i < healthBars.Count; i++)
            {
                int barMax = _barValues[i];
                int barValue = Mathf.Clamp(healthRemaining, 0, barMax);

                float scaleX = (float)barValue / barMax;
                healthBars[i].rectTransform.localScale = new Vector3(scaleX, 1f, 1f);

                healthRemaining -= barValue;
            }
        }
        
        /// <summary>
        /// Initializes bar values based on total health.
        /// </summary>
        private void SetupBars()
        {
            if (healthBars.Count != 3)
            {
                Debug.LogError("Exactly 3 health bars are required.");
                return;
            }

            _healthPerBar = _totalHealth / 3;
            _barValues = new List<int> { _healthPerBar, _healthPerBar, _healthPerBar };

            // Distribute any remainder to the last bar
            int remainder = _totalHealth % 3;
            _barValues[2] += remainder;

            // Initialize each bar to scale (0, 1, 1) — empty bar
            foreach (Image bar in healthBars)
            {
                bar.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            }
        }

        private IEnumerator FillBarsSequentially()
        {
            for (int i = 0; i < healthBars.Count; i++)
            {
                const float duration = 0.5f;
                float elapsed = 0f;

                // Target scale is proportional to how full the bar should be (0 to 1)
                float targetScaleX = Mathf.Clamp01((float)_barValues[i] / _healthPerBar);

                Vector3 initialScale = new Vector3(0f, 1f, 1f);
                Vector3 targetScale = new Vector3(targetScaleX, 1f, 1f);

                // Set initial scale to zero (empty)
                healthBars[i].rectTransform.localScale = initialScale;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);

                    healthBars[i].rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
                    yield return null;
                }

                healthBars[i].rectTransform.localScale = targetScale;
            }
        }
    }
}
