using UnityEngine;

namespace Vampire
{
    public class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance { get; private set; }

        public event System.Action<int> OnCoinsChanged;

        private int _coins;
        public int Coins
        {
            get => _coins;
            private set
            {
                _coins = value;
                PlayerPrefs.SetInt("TotalCoins", _coins);
                PlayerPrefs.Save();
                OnCoinsChanged?.Invoke(_coins);
            }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _coins = PlayerPrefs.GetInt("TotalCoins", 0);
        }

        public void AddCoins(int amount) => Coins += amount;

        public bool SpendCoins(int amount)
        {
            if (_coins < amount) return false;
            Coins -= amount;
            return true;
        }

        // Útil para resetear al iniciar una nueva partida si lo necesitas
        public void ResetCoins()
        {
            Coins = 0;
        }
    }
}
