using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InferenceResponse
{
    public int num_inf;
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

[Serializable]
public class HistoryResponse
{
    public List<InferenceHistoryItem> history;
}

[Serializable]
public class InferenceHistoryItem
{
    public int id;
    public string predictions;
    public int image_width;
    public int image_height;
    public string created_at;
    public int user_id;

    public List<Prediction> GetParsedPredictions()
    {
        string wrapped = $"{{\"items\":{predictions}}}";
        return JsonUtility.FromJson<PredictionWrapper>(wrapped).items;
    }

    [Serializable]
    private class PredictionWrapper
    {
        public List<Prediction> items;
    }
}
