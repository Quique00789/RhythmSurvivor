using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Vampire
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Referencias UI")]
        public GameObject shopPanel;
        public Transform skillContainer;
        public GameObject skillItemPrefab;
        public TextMeshProUGUI coinDisplay;

        [Header("Habilidades disponibles")]
        public List<SkillData> allSkills;

        void OnEnable()
        {
            CoinManager.Instance.OnCoinsChanged += UpdateCoins;
            UpdateCoins(CoinManager.Instance.Coins);
            PopulateShop();
        }

        void OnDisable()
        {
            CoinManager.Instance.OnCoinsChanged -= UpdateCoins;
        }

        void UpdateCoins(int amount)
        {
            coinDisplay.text = $"Monedas: {amount}";
        }

        void PopulateShop()
        {
            foreach (Transform child in skillContainer)
                Destroy(child.gameObject);

            foreach (var skill in allSkills)
            {
                var item = Instantiate(skillItemPrefab, skillContainer);
                bool owned = SkillInventory.Instance.HasSkill(skill.skillId);

                item.transform.Find("SkillName").GetComponent<TextMeshProUGUI>().text = skill.displayName;
                item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = skill.description;
                item.transform.Find("Icon").GetComponent<Image>().sprite = skill.icon;

                var costText = item.transform.Find("CostText").GetComponent<TextMeshProUGUI>();
                costText.text = owned ? "- Comprado -" : $"{skill.cost} monedas";

                var btn = item.transform.Find("BuyButton").GetComponent<Button>();
                btn.interactable = !owned;

                var skillRef = skill;
                btn.onClick.AddListener(() => TryBuy(skillRef));
            }
        }

        void TryBuy(SkillData skill)
        {
            if (SkillInventory.Instance.HasSkill(skill.skillId)) return;

            if (CoinManager.Instance.SpendCoins(skill.cost))
            {
                SkillInventory.Instance.Unlock(skill.skillId);
                PopulateShop();
            }
            else
            {
                Debug.Log("Monedas insuficientes");
            }
        }

        public void OpenShop()
        {
            shopPanel.SetActive(true);

            RectTransform rect = shopPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            
            // Fuerza el tamaño exacto del Canvas padre
            RectTransform canvasRect = shopPanel.transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = Vector2.zero;
        }

        public void CloseShop()
        {
            shopPanel.SetActive(false);
        }
    }
}
