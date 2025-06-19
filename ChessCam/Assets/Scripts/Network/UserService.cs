using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

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
                string raw = request.downloadHandler.text;
                string details = raw.Split(":\"")[1].TrimEnd('}', '"');
                Debug.LogError("Details: " + details);
                NotificationUI.Instance.ShowMessage(details, true);
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
                NotificationUI.Instance.ShowMessage("Login Success", false);
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

}
