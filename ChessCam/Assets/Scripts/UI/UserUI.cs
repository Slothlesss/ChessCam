using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserUI : MonoBehaviour
{
    [Header("Register Input Fields")]
    public TMP_InputField usernameRegisterInput;
    public TMP_InputField passwordRegisterInput;
    public TMP_InputField confirmPasswordInput;

    [Header("Login Input Fields")]
    public TMP_InputField usernameLoginInput;
    public TMP_InputField passwordLoginInput;

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
        string username = usernameRegisterInput.text;
        string password = passwordRegisterInput.text;
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
        string username = usernameLoginInput.text;
        string password = passwordLoginInput.text;
        StartCoroutine(UserService.LoginUser(username, password));
    }
}
