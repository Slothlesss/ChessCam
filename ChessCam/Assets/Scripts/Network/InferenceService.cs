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

    public InferenceResponse inferenceResult;
    public HistoryResponse historyResult;

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void UploadFile();
#endif
    
    // ========== Button Functions ==========
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
    public void Inference()
    {
        StartCoroutine(SendImageForInference());
    }

    public void GetHistory()
    {
        StartCoroutine(GetInferenceHistory());
    }


    // ========== End: Button Functions ==========

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

    IEnumerator SendImageForInference()
    {
        byte[] imageBytes = image.EncodeToJPG();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", imageBytes, "image.jpg", "image/jpeg"),
            new MultipartFormDataSection("image_width", image.width.ToString()),
            new MultipartFormDataSection("image_height", image.height.ToString()),
            new MultipartFormDataSection("user_id", UserSession.userId.ToString())
        };

        int attempt = 0;
        bool success = false;
        int maxRetries = 3;
        float retryDelay = 1f;

        while (attempt < maxRetries && !success)
        {
            attempt++;

            UnityWebRequest request = UnityWebRequest.Post(APIConfig.Inference.Detect, formData);
            request.SetRequestHeader("Accept", "application/json");

            NotificationUI.Instance.StartLoadingMessage("Analyzing");

            yield return request.SendWebRequest(); 

            if (request.result == UnityWebRequest.Result.Success)
            {
                success = true;
                string result = request.downloadHandler.text;

                inferenceResult = JsonUtility.FromJson<InferenceResponse>(result);
                if (inferenceResult.num_inf >= 3)
                {
                    Debug.Log(result);
                    NotificationUI.Instance.ShowMessage("You have reached your daily inference limit", true);
                }
                else
                {
                    inferenceResult.predictions = inferenceResult.predictions
                        .Where(pred => pred.confidence >= 0.8f)
                        .ToArray();

                    NotificationUI.Instance.ShowMessage("Successfully detect. You can spawn now.", false);
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

    public IEnumerator GetInferenceHistory()
    {
        string url = $"{APIConfig.Inference.History}?user_id={UnityWebRequest.EscapeURL(UserSession.userId.ToString())}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ParseHistoryJson(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Upload failed: " + request.error);
            Debug.LogError("Server response: " + request.downloadHandler.text);
        }
    }
    private void ParseHistoryJson(string json)
    {
        historyResult = JsonUtility.FromJson<HistoryResponse>(json);

        foreach (var item in historyResult.history)
        {
            List<Prediction> preds = item.GetParsedPredictions();

            Debug.Log($"Inference #{item.id} with {preds.Count} predictions at {item.created_at}");
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
