using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Diagnostics;

public class RoboflowUploader : Singleton<RoboflowUploader>
{
    [SerializeField] private string modelId = "chessdetection-uhtoo/1"; // Replace with your model ID
    [SerializeField] private string apiKey = "454LyvkAPO9ikEV3oHnm";    // Replace with your API key
    [SerializeField] private Texture2D image; // Assign your image in the Unity Inspector

    public InferenceResponse result;
    public void GetResponseFromRoboflow()
    {
        StartCoroutine(SendImageForInference(image));
    }

    IEnumerator SendImageForInference(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToJPG(); // Or .EncodeToPNG() based on your image format
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

        foreach (var pred in result.predictions)
        {
            UnityEngine.Debug.Log($"Class: {pred.@class}, Box: ({pred.x}, {pred.y}), Confidence: {pred.confidence}");
        }
    }
}
