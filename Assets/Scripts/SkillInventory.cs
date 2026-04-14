using UnityEngine;
using System.Collections.Generic;

namespace Vampire
{
    public class SkillInventory : MonoBehaviour
    {
        public static SkillInventory Instance { get; private set; }
        private HashSet<string> _unlocked = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        public bool HasSkill(string skillId) => _unlocked.Contains(skillId);

        public void Unlock(string skillId)
        {
            _unlocked.Add(skillId);
            Save();
        }

        void Save()
        {
            PlayerPrefs.SetString("UnlockedSkills", string.Join(",", _unlocked));
            PlayerPrefs.Save();
        }

        void Load()
        {
            string data = PlayerPrefs.GetString("UnlockedSkills", "");
            if (!string.IsNullOrEmpty(data))
                _unlocked = new HashSet<string>(data.Split(','));
        }
    }
}
