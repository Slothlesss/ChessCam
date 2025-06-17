[System.Serializable]
public class InferenceResponse
{
    public Prediction[] predictions;
}

[System.Serializable]
public class Prediction
{
    public string name;
    public float x;
    public float y;
    public float confidence;
}
