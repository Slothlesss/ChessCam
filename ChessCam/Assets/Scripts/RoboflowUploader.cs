using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using SFB;

public class RoboflowUploader : Singleton<RoboflowUploader>
{
    [SerializeField] private string modelId = "chessdetection-uhtoo/1"; // Replace with model ID
    [SerializeField] private string apiKey = "454LyvkAPO9ikEV3oHnm";    // Replace with API key
    [SerializeField] private Texture2D image;

    public InferenceResponse result;

    public void PickImage()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Choose an image", "", new[] { new ExtensionFilter("Images", "png", "jpg", "jpeg") }, false);
        if (paths.Length > 0 && File.Exists(paths[0]))
        {
            StartCoroutine(LoadTextureFromPath(paths[0]));
        }
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
    public void GetResponseFromRoboflow()
    {
        StartCoroutine(SendImageForInference(image));
    }

    IEnumerator SendImageForInference(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToJPG(); // Or .EncodeToPNG() based on image format
        string base64Image = System.Convert.ToBase64String(imageBytes);
        byte[] postData = Encoding.ASCII.GetBytes(base64Image);

        string url = $"https://detect.roboflow.com/{modelId}?api_key={apiKey}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log("Result:\n" + request.downloadHandler.text);
        }
        else
        {
            UnityEngine.Debug.LogError("Error: " + request.error);
        }

        result = JsonUtility.FromJson<InferenceResponse>(request.downloadHandler.text);
        result.predictions = result.predictions.Where(pred => pred.confidence >= 0.8f).ToArray();
        foreach (var pred in result.predictions)
        {
            UnityEngine.Debug.Log($"Class: {pred.@class}, Box: ({pred.x}, {pred.y}), Confidence: {pred.confidence}");
        }
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
