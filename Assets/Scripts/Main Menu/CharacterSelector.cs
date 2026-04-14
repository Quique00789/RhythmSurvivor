using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Vampire
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField] protected CharacterBlueprint[] characterBlueprints;
        [SerializeField] protected GameObject characterCardPrefab;
        [SerializeField] protected CoinDisplay coinDisplay;
        [SerializeField] private GameObject levelSelector; // Agrega esto

        private CharacterCard[] characterCards;
        private int selectedLevel = 1;

        private void OnEnable()
        {
            // Reset state when the menu is opened
            if (levelSelector != null)
                levelSelector.SetActive(false);

            if (characterCards != null)
            {
                for (int i = 0; i < characterCards.Length; i++)
                {
                    if (characterCards[i] != null)
                        characterCards[i].gameObject.SetActive(true);
                }
            }
        }

        public void Init()
        {
            if (levelSelector != null)
                levelSelector.SetActive(false); // Hide level selector at start

            characterCards = new CharacterCard[characterBlueprints.Length];
            for (int i = 0; i < characterBlueprints.Length; i++)
            {
                characterCards[i] = Instantiate(characterCardPrefab, this.transform).GetComponent<CharacterCard>();
                characterCards[i].Init(this, characterBlueprints[i], coinDisplay);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            for (int i = 0; i < characterBlueprints.Length; i++)
            {
                characterCards[i].UpdateLayout();
            }
        }

        public void SetLevel(int levelIndex)
        {
            selectedLevel = levelIndex;
            // Oculta los botones al seleccionar nivel
            if (levelSelector != null)
                levelSelector.SetActive(false);

            // Start the game now that level is selected
            SceneManager.LoadScene(selectedLevel);
        }

        public void StartGame(CharacterBlueprint characterBlueprint)
        {
            // Instead of starting the game, show the level selector
            CrossSceneData.CharacterBlueprint = characterBlueprint;
            
            // Hide character cards so it looks like a new menu
            for (int i = 0; i < characterCards.Length; i++)
            {
                if (characterCards[i] != null)
                    characterCards[i].gameObject.SetActive(false);
            }

            if (levelSelector != null)
                levelSelector.SetActive(true);
        }

        // Método para el botón de regresar/cerrar (la tachita)
        public void GoBack()
        {
            // Si el selector de niveles está activo, regresamos a la selección de personaje
            if (levelSelector != null && levelSelector.activeSelf)
            {
                levelSelector.SetActive(false);
                for (int i = 0; i < characterCards.Length; i++)
                {
                    if (characterCards[i] != null)
                        characterCards[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // Si estamos en la selección de personajes, cerramos el menú entero
                this.gameObject.SetActive(false);
            }
        }
    }
}