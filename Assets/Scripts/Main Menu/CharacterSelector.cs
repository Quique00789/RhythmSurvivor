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

        public void Init()
        {
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
        }

        public void StartGame(CharacterBlueprint characterBlueprint)
        {
            CrossSceneData.CharacterBlueprint = characterBlueprint;
            SceneManager.LoadScene(selectedLevel);
        }
    }
}