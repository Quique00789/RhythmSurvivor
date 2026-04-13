using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vampire
{
    public class LoginUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;

        [Header("Login")]
        [SerializeField] private TMP_InputField loginUsername;
        [SerializeField] private TMP_InputField loginPassword;
        [SerializeField] private Button loginButton;
        [SerializeField] private TMP_Text loginErrorText;

        [Header("Register")]
        [SerializeField] private TMP_InputField registerUsername;
        [SerializeField] private TMP_InputField registerPassword;
        [SerializeField] private TMP_InputField registerPasswordConfirm;
        [SerializeField] private Button registerButton;
        [SerializeField] private TMP_Text registerErrorText;

        [Header("Referencias")]
        [SerializeField] private GameObject mainMenuUI; // El GameObject de tu menú principal

        private void Start()
        {
            StartCoroutine(InitAuth());
        }

        private IEnumerator InitAuth()
        {
            yield return AuthManager.Instance.InitializeAsync().AsCoroutine();

            // Si ya tiene sesión activa, saltar login directo
            if (AuthManager.Instance.IsSignedIn)
            {
                OpenMainMenu();
            }
            else
            {
                loginPanel.SetActive(true);
                registerPanel.SetActive(false);
            }
        }

        public void OnLoginPressed()
        {
            StartCoroutine(DoLogin());
        }

        private IEnumerator DoLogin()
        {
            loginButton.interactable = false;
            loginErrorText.text = "";

            var task = AuthManager.Instance.LoginAsync(loginUsername.text, loginPassword.text);
            yield return task.AsCoroutine();

            if (task.Result)
            {
                OpenMainMenu();
            }
            else
            {
                loginErrorText.text = "Usuario o contraseña incorrectos.";
                loginButton.interactable = true;
            }
        }

        public void OnRegisterPressed()
        {
            StartCoroutine(DoRegister());
        }

        private IEnumerator DoRegister()
        {
            registerErrorText.text = "";

            // Validar contraseñas iguales
            if (registerPassword.text != registerPasswordConfirm.text)
            {
                registerErrorText.text = "Las contraseñas no coinciden.";
                yield break;
            }

            // Validar requisitos antes de enviar al servidor
            string passwordError = ValidatePassword(registerPassword.text);
            if (passwordError != null)
            {
                registerErrorText.text = passwordError;
                yield break;
            }

            registerButton.interactable = false;

            var task = AuthManager.Instance.RegisterAsync(registerUsername.text, registerPassword.text);
            yield return task.AsCoroutine();

            if (task.Result)
            {
                OpenMainMenu();
            }
            else
            {
                registerErrorText.text = "No se pudo crear la cuenta. Intenta con otro usuario.";
                registerButton.interactable = true;
            }
        }

        private string ValidatePassword(string password)
        {
            if (password.Length < 8)
                return "Mínimo 8 caracteres.";

            bool tieneUpper = false, tieneLower = false, tieneNumero = false, tieneSimbolo = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) tieneUpper = true;
                else if (char.IsLower(c)) tieneLower = true;
                else if (char.IsDigit(c)) tieneNumero = true;
                else tieneSimbolo = true;
            }

            if (!tieneUpper)   return "Necesita al menos una mayúscula.";
            if (!tieneLower)   return "Necesita al menos una minúscula.";
            if (!tieneNumero)  return "Necesita al menos un número.";
            if (!tieneSimbolo) return "Necesita al menos un símbolo (!@#$%).";

            return null; // Todo bien
        }

        public void ShowRegisterPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }

        public void ShowLoginPanel()
        {
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }

        private void OpenMainMenu()
        {
            this.gameObject.SetActive(false);    // Oculta el panel de login
            mainMenuUI.SetActive(true);          // Muestra tu menú principal
        }
    }
}
