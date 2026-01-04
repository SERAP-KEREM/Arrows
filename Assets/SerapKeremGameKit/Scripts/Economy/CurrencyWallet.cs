using SerapKeremGameKit._Singletons;
using UnityEngine;
using SerapKeremGameKit._Utilities;

namespace SerapKeremGameKit._Economy
{
    public sealed class CurrencyWallet : MonoSingleton<CurrencyWallet>
    {
        private const string CoinsKey = PreferencesKeys.WalletCoins;
        private const string CoinsSigKey = PreferencesKeys.WalletCoinsSig;
        private const int Salt = 739391;

        public int Coins { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Load();
        }

        public void Load()
        {
            int saved = PlayerPrefs.GetInt(CoinsKey, 0);
            int sig = PlayerPrefs.GetInt(CoinsSigKey, 0);
            Coins = sig == ComputeSig(saved) ? saved : 0;
        }

        public void Save()
        {
            PlayerPrefs.SetInt(CoinsKey, Coins);
            PlayerPrefs.SetInt(CoinsSigKey, ComputeSig(Coins));
            SaveUtility.SaveDebounced();
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            Coins += amount;
            Save();
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            if (Coins < amount) return false;
            Coins -= amount;
            Save();
            return true;
        }

        private int ComputeSig(int value)
        {
            unchecked { return (value ^ Salt) * 397; }
        }
    }
}



