using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour, IUiReadable
{
    public FadeInfo[] fadeObjects;
    public float fadeTime;
    public bool fadeLock;


    private bool isFading;

    public void FadeIn()
    {
        if (isFading)
            return;

        for (int i = 0; i < fadeObjects.Length; i++)
        {
            StartCoroutine(Fade(fadeObjects[i].image, fadeObjects[i].text, fadeObjects[i].maxOpacity, true));
        }
    }

    public void FadeOut()
    {
        if (isFading)
            return;

        for (int i = 0; i < fadeObjects.Length; i++)
        {
            StartCoroutine(Fade(fadeObjects[i].image, fadeObjects[i].text, fadeObjects[i].maxOpacity, false));
        }
    }

    private IEnumerator Fade(Image image, TextMeshProUGUI text, float maxOpacity, bool fadeIn)
    {
        isFading = true;

        float passedTime = fadeIn ? 0f : 1f;
        Color savedColour;

        while (true)
        {
            passedTime += fadeIn ? (Time.deltaTime / fadeTime) : -(Time.deltaTime / fadeTime);

            if (image != null)
            {
                savedColour = image.color;
                image.color = new Color(savedColour.r, savedColour.g, savedColour.b, Mathf.Clamp(passedTime * maxOpacity, 0f, maxOpacity));
            }
            if (text != null)
            {
                savedColour = text.color;
                text.color = new Color(savedColour.r, savedColour.g, savedColour.b, Mathf.Clamp(passedTime * maxOpacity, 0f, maxOpacity));
            }
            yield return null;

            if (fadeIn && passedTime >= 1)
                break;
            if (!fadeIn && passedTime <= 0)
                break;
        }

        if (!fadeLock)
            isFading = false;
    }

    public InfoStore GetInfo()
    {
        return null;
    }

    public void Activate()
    {
        FadeIn();
    }


    [Serializable]
    public class FadeInfo
    {
        public Image image;
        public TextMeshProUGUI text;
        public float maxOpacity;
    }
}
