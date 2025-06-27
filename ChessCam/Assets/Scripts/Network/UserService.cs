using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public static class UserService
{
    public static IEnumerator RegisterUser(string username, string password)
    {
        string url = $"{APIConfig.Users.Register}?username={UnityWebRequest.EscapeURL(username)}&password={UnityWebRequest.EscapeURL(password)}";

        NotificationUI.Instance.StartLoadingMessage("Signing up");

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Success: " + request.downloadHandler.text);
            NotificationUI.Instance.ShowMessage("Successfully register. You can login now.", false);
        }
        else
        {
            Debug.LogError("Error: " + request.responseCode + " - " + request.error);
            string raw = request.downloadHandler.text;
            string details = raw.Split(":\"")[1].TrimEnd('}', '"');
            Debug.LogError("Details: " + details);
            NotificationUI.Instance.ShowMessage(details, true);
        }

    }

    public static IEnumerator LoginUser(string username, string password)
    {
        string url = $"{APIConfig.Users.Login}?username={UnityWebRequest.EscapeURL(username)}&password={UnityWebRequest.EscapeURL(password)}";

        NotificationUI.Instance.StartLoadingMessage("Logging in");

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Success: " + request.downloadHandler.text);
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            UserSession.userId = response.id;

            NotificationUI.Instance.ShowMessage("Login Success", false);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            Debug.LogError("Error: " + request.responseCode + " - " + request.error);
            string raw = request.downloadHandler.text;
            string details = raw.Split(":\"")[1].TrimEnd('}', '"');
            Debug.LogError("Details: " + details);
            NotificationUI.Instance.ShowMessage(details, true);
        }
    }

}
