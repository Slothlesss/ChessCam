using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class UserService
{
    public static IEnumerator RegisterUser(string username, string password)
    {
        string url = $"{APIConfig.Users.Register}?username={UnityWebRequest.EscapeURL(username)}&password={UnityWebRequest.EscapeURL(password)}";

        NotificationUI.Instance.StartLoadingMessage("Registering");
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
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
                Debug.LogError("Body: " + request.downloadHandler.text);
                NotificationUI.Instance.ShowMessage("Server errors. Please try again later.", true);
            }
        }
    }

    public static IEnumerator LoginUser(string username, string password)
    {
        string url = $"{APIConfig.Users.Login}?username={UnityWebRequest.EscapeURL(username)}&password={UnityWebRequest.EscapeURL(password)}";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.responseCode + " - " + request.error);
                Debug.LogError("Body: " + request.downloadHandler.text);
            }
        }
    }

}
