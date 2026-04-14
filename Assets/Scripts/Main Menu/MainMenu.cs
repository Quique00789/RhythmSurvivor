using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CharacterSelector characterSelector;
        [SerializeField] private GameObject loginUI; // Referencia al LoginUI

        void Start()
        {
            characterSelector.Init();
        }

        public void OpenLogin()
        {
            loginUI.SetActive(true);
        }
    }
}
