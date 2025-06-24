using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotHelper : Singleton<ScreenshotHelper>
{
    public Texture2D screenshot;
    public IEnumerator CaptureScreenshot(Action<string> onComplete)
    {
        yield return new WaitForEndOfFrame();

        screenshot = new Texture2D(800, 800, TextureFormat.RGB24, false);

        int x = Screen.width / 2 - 400;
        int y = Screen.height / 2 - 400;

        screenshot.ReadPixels(new Rect(x, y, 800, 800), 0, 0);
        screenshot.Apply();


        byte[] bytes = screenshot.EncodeToPNG();

        string tempPath = System.IO.Path.Combine(Application.temporaryCachePath, "screenshot.png");
        System.IO.File.WriteAllBytes(tempPath, bytes);
        Debug.Log("Screenshot saved to: " + tempPath);
    }

    public Texture2D Screenshot()
    {
        StartCoroutine(CaptureScreenshot(null));
        return screenshot;
    }
}