using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class APIConfig
{
    public static string BaseUrl = "https://chesscamserver-production.up.railway.app";

    // Endpoints
    public static string Health => $"{BaseUrl}/health";
    public static class Users
    {
        public static string Register => $"{BaseUrl}/users/register";
        public static string Login => $"{BaseUrl}/users/login";
    }
    public static class Inference
    {
        public static string Detect => $"{BaseUrl}/inferences/detect";
        public static string Save => $"{BaseUrl}/inferences/save";
        public static string History => $"{BaseUrl}/inferences/history";
    }
}
