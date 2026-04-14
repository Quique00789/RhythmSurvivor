using UnityEngine;
using UnityEngine.UI;

namespace Vampire
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Image pauseButton;
        [SerializeField] private Sprite pauseSprite, playSprite;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource kickSource;
        private bool paused = false;
        private bool timeIsFrozen = false;

        public bool TimeIsFrozen { set => timeIsFrozen = value; }

        public void PlayPause()
        {
            if (paused = !paused)
            {
                if (!timeIsFrozen)
                    Time.timeScale = 0;
                // Pause music and rhythm notes to maintain sync
                if (musicSource != null)
                    musicSource.Pause();
                if (kickSource != null)
                    kickSource.Pause();
                if (RhythmNotes.Instance != null)
                    RhythmNotes.Instance.SetPaused(true);
                pauseButton.sprite = playSprite;
                pauseMenu.SetActive(true);
            }
            else
            {
                if (!timeIsFrozen)
                    Time.timeScale = 1;
                // Resume music and rhythm notes from the exact same point
                if (musicSource != null)
                    musicSource.UnPause();
                if (kickSource != null)
                    kickSource.UnPause();
                if (RhythmNotes.Instance != null)
                    RhythmNotes.Instance.SetPaused(false);
                pauseButton.sprite = pauseSprite;
                pauseMenu.SetActive(false);
            }
        }
    }
}
