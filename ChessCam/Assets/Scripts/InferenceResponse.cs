[System.Serializable]
public class InferenceResponse
{
    public string inference_id;
    public float time;
    public ImageData image;
    public Prediction[] predictions;
}

[System.Serializable]
public class ImageData
{
    public int width;
    public int height;
}

[System.Serializable]
public class Prediction
{
    public float x;
    public float y;
    public float width;
    public float height;
    public float confidence;
    public string @class; // 'class' is a reserved keyword in C#
    public int class_id;
    public string detection_id;
}
