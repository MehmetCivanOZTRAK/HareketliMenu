using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class QuitWarning : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 5f;

    private float targetAlpha = 0f;

    private void Update()
    {
        if (canvasGroup == null) return;
        
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
 
    }

    public void ShowQuitWarning()
    {
        targetAlpha = 1f;
    }
    public void HideQuitWarning()
    {
        targetAlpha = 0f;
    }
}
