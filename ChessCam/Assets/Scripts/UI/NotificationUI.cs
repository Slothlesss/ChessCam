using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : Singleton<NotificationUI>
{
    [Header("Notification")]
    [SerializeField] private CanvasGroup notification;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI messageUI;
    [SerializeField] private Sprite errorBackground;
    [SerializeField] private Sprite successBackground;
    [SerializeField] private Sprite loadingBackground;

    [Header("Banner")]
    [SerializeField] private GameObject winBanner;
    [SerializeField] private GameObject loseBanner;


    [Header("Timing")]
    public float fadeDuration = 0.2f;
    public float displayDuration = 1f;

    private Coroutine currentRoutine;
    private void Start()
    {
        notification.gameObject.SetActive(false);
    }

    public void ShowMessage(string message, bool isError)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message, isError));
    }

    public void StartLoadingMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(LoadingDotsRoutine(message));
    }

    public void ShowEndGameBanner(bool isWhiteWin)
    {
        ChessPiece whiteKing = ChessRules.FindKing(true);
        ChessPiece blackKing = ChessRules.FindKing(false);
        GameObject win = Instantiate(winBanner, isWhiteWin ? whiteKing.transform : blackKing.transform);
        win.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, 50);

        GameObject lose = Instantiate(loseBanner, isWhiteWin ? blackKing.transform : whiteKing.transform);
        lose.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, 50);
    }

    private IEnumerator ShowRoutine(string message, bool isError)
    {
        notification.gameObject.SetActive(true);
        messageUI.text = message;
        if (isError)
        {
            background.sprite = errorBackground;

        }
        else
        {
            background.sprite = successBackground;
        }

        // Fade in
        yield return FadeEffect(notification, 0f, 1f, fadeDuration);

        // Wait
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return FadeEffect(notification, 1f, 0f, fadeDuration);

        notification.gameObject.SetActive(false);
    }

    public IEnumerator StopLoadingMessage()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        notification.alpha = 1f;
        messageUI.text = $"Loading complete.";
        yield return AutoFadeOut();
    }
    private IEnumerator LoadingDotsRoutine(string message)
    {
        notification.gameObject.SetActive(true);
        background.sprite = loadingBackground;
        notification.alpha = 1f;

        string baseText = $"{message}";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4; // 0 to 3
            messageUI.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator AutoFadeOut()
    {
        yield return new WaitForSeconds(displayDuration);
        yield return FadeEffect(notification, 1f, 0f, fadeDuration);
        notification.gameObject.SetActive(false);
    }

    private IEnumerator FadeEffect(CanvasGroup notification, float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            notification.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }
        notification.alpha = end;
    }
}
