using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;

    [Header("Buttons")]
    public Button registerButton;
    public Button loginButton;

    private void Start()
    {
        registerButton.onClick.AddListener(OnRegisterClicked);
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnRegisterClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password != confirmPassword)
        {
            NotificationUI.Instance.ShowMessage("Passwords do not match", true);
            return;
        }

        StartCoroutine(UserService.RegisterUser(username, password));
    }

    private void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(UserService.LoginUser(username, password));
    }
}
