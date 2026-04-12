using UnityEngine;
namespace Vampire
{
    [CreateAssetMenu(menuName = "Roguelite/SkillData")]
    public class SkillData : ScriptableObject
    {
        public string skillId;
        public string displayName;
        [TextArea] public string description;
        public int cost;
        public Sprite icon;
    }
}
