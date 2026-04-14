using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Vampire
{
    /// <summary>
    /// Visual feedback for successful dashes: screen flash + optional bloom pulse.
    /// 
    /// The FLASH works immediately (no packages needed):
    ///   - Uses a full-screen UI Image with additive blending
    ///   - Flashes white/gold on hit, fades out smoothly
    /// 
    /// The BLOOM requires Post Processing Stack v2 (see setup instructions):
    ///   - Temporarily increases bloom intensity on hit
    ///   - Fades back to base intensity
    /// 
    /// Call TriggerEffect(intensity) from RhythmDash on Perfect/Good hits.
    /// </summary>
    public class DashVisualFeedback : MonoBehaviour
    {
        public static DashVisualFeedback Instance;

        [Header("Screen Flash")]
        [Tooltip("Full-screen UI Image for flash effect. Must use Additive shader.")]
        [SerializeField] private Image flashImage;

        [Tooltip("Flash color for Perfect hits.")]
        [SerializeField] private Color perfectColor = new Color(1f, 0.9f, 0.5f, 0.35f);

        [Tooltip("Flash color for Good hits.")]
        [SerializeField] private Color goodColor = new Color(1f, 1f, 1f, 0.2f);

        [Tooltip("How fast the flash fades out. Higher = shorter flash.")]
        [SerializeField] private float flashFadeSpeed = 5f;

        // Internal
        private Coroutine activeFlash;

        void Awake()
        {
            Instance = this;
            // Start fully transparent
            if (flashImage != null)
            {
                Color c = flashImage.color;
                c.a = 0f;
                flashImage.color = c;
            }
        }

        /// <summary>
        /// Triggers the visual effect.
        /// Call with true for Perfect, false for Good.
        /// </summary>
        public void TriggerEffect(bool isPerfect)
        {
            // Screen flash
            if (flashImage != null)
            {
                if (activeFlash != null)
                    StopCoroutine(activeFlash);
                activeFlash = StartCoroutine(FlashCoroutine(isPerfect ? perfectColor : goodColor));
            }
        }

        private IEnumerator FlashCoroutine(Color color)
        {
            flashImage.color = color;

            float alpha = color.a;

            while (alpha > 0.01f)
            {
                alpha = Mathf.Lerp(alpha, 0f, flashFadeSpeed * Time.unscaledDeltaTime);
                flashImage.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            flashImage.color = new Color(color.r, color.g, color.b, 0f);
            activeFlash = null;
        }
    }
}
