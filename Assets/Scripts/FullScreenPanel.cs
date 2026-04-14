using UnityEngine;

namespace Vampire
{
    public class FullScreenPanel : MonoBehaviour
    {
        void Start()
        {
            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
