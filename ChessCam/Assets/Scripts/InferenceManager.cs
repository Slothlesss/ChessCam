using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class InferenceManager : Singleton<InferenceManager>
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
    public void GetResponseFromMyServer()
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

        string url = "https://chesscamserver-production.up.railway.app/detect";

        int attempt = 0;
        bool success = false;

        while (attempt < maxRetries && !success)
        {
            attempt++;

            UnityWebRequest request = UnityWebRequest.Post(url, formData);
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                success = true;

                Debug.Log($"Success (attempt {attempt}):\n{request.downloadHandler.text}");

                string wrappedJson = "{\"predictions\":" + request.downloadHandler.text + "}";

                result = JsonUtility.FromJson<InferenceResponse>(wrappedJson);
                result.predictions = result.predictions
                    .Where(pred => pred.confidence >= 0.8f)
                    .ToArray();

                foreach (var pred in result.predictions)
                {
                    Debug.Log($"Class: {pred.name}, Box: ({pred.x}, {pred.y}), Confidence: {pred.confidence}");
                }
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
                    Debug.LogError("All retries failed.");
                }
            }
        }
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
