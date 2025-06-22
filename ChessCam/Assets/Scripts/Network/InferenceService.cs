using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class InferenceService : Singleton<InferenceService>
{
    [SerializeField] private Texture2D image;

    public InferenceResponse result;

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void UploadFile();
#endif

    public void PickImage()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(); // from JS plugin
#else
        var paths = StandaloneFileBrowser.OpenFilePanel(
            "Choose an image", "", new[] { new ExtensionFilter("Images", "png", "jpg", "jpeg") }, false);

        if (paths.Length > 0 && File.Exists(paths[0]))
        {
            StartCoroutine(LoadTextureFromPath(paths[0]));
        }
#endif
    }


    private IEnumerator LoadTextureFromPath(string path)
    {
        string uri = "file://" + path;

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(uri))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load texture: {uwr.error}");
            }
            else
            {
                image = DownloadHandlerTexture.GetContent(uwr);
            }
        }
    }
    public void Inference()
    {
        StartCoroutine(SendImageForInference());
    }

    IEnumerator SendImageForInference(int maxRetries = 3, float retryDelay = 1f)
    {
        byte[] imageBytes = image.EncodeToJPG();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", imageBytes, "image.jpg", "image/jpeg")
        };

        int attempt = 0;
        bool success = false;

        while (attempt < maxRetries && !success)
        {
            attempt++;

            UnityWebRequest request = UnityWebRequest.Post(APIConfig.Inference.DetectEndpoint, formData);
            request.SetRequestHeader("Accept", "application/json");

            NotificationUI.Instance.StartLoadingMessage("Requesting");

            yield return request.SendWebRequest(); 

            if (request.result == UnityWebRequest.Result.Success)
            {
                success = true;

                string wrappedJson = "{\"predictions\":" + request.downloadHandler.text + "}";

                result = JsonUtility.FromJson<InferenceResponse>(wrappedJson);
                result.predictions = result.predictions
                    .Where(pred => pred.confidence >= 0.8f)
                    .ToArray();

                NotificationUI.Instance.ShowMessage("Successfully inference. You can spawn now.", false);
            }
            else
            {
                Debug.LogWarning($"Attempt {attempt} failed: {request.error}");

                if (attempt < maxRetries)
                {
                    Debug.Log($"Retrying in {retryDelay} seconds...");
                    yield return new WaitForSeconds(retryDelay);
                }
                else
                {
                    StartCoroutine(StopThenShowError());
                }
            }
        }
    }

    private IEnumerator StopThenShowError()
    {
        yield return NotificationUI.Instance.StopLoadingMessage();
        NotificationUI.Instance.ShowMessage("Server errors. Please try again later.", true);
    }


    public void ReceiveImageData(string base64)
    {
        Debug.Log("Received image from WebGL");

        base64 = base64.Substring(base64.IndexOf(",") + 1); // remove "data:image/jpeg;base64,..."
        byte[] bytes = Convert.FromBase64String(base64);

        image = new Texture2D(2, 2);
        image.LoadImage(bytes);

        Debug.Log("Image loaded into Texture2D");
    }


    public int GetImageWidth()
    {
        return image.width;
    }

    public int GetImageHeight()
    {
        return image.height;
    }
}
