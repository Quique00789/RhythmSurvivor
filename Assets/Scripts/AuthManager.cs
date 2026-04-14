using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

namespace Vampire
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public async Task InitializeAsync()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
                Debug.Log($"Sesión iniciada. PlayerID: {AuthenticationService.Instance.PlayerId}");

            AuthenticationService.Instance.SignInFailed += (err) =>
                Debug.LogError($"Error: {err.Message}");
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                return true;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError($"Login fallido: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                return true;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError($"Registro fallido: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            AuthenticationService.Instance.SignOut();
        }

        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
    }
}
