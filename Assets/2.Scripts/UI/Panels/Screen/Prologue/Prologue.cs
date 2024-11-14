using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Prologue : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI upperText;
    [SerializeField] private TextMeshProUGUI underText;
    [SerializeField] private Image upperTextBar;
    [SerializeField] private Image underTextBar;
    [SerializeField] private Image upperImage;
    [SerializeField] private Image underImage;
    [SerializeField] private Button panelButton;

    public float delay = 0.025f;
    private bool isRevealing = true;
    private bool isFullTextShown = false;
    private string[] texts;

    private int count = 0;

    private float elapsedTime = 0f;

    private WaitForSeconds delayWaitForSeconds;

    [SerializeField] private float imageDuration = 1f;
    [SerializeField] private float panelDelay = 0.5f;


    [SerializeField] private Sprite[] prologueSprites;

    private bool isPrologueEnded;

    public event Action OnPrologueEnded;

    public void Init()
    {
        skipButton.onClick.AddListener(GoToMainScene);
        panelButton.onClick.AddListener(SkipCurrentDialogue);
        panelButton.gameObject.SetActive(true);
        TextAsset textAsset = Resources.Load<TextAsset>("CSV/Prologue/SpartaPrologue CSV");
        string[] rows = textAsset.text.Split('\n');
        texts = new string[rows.Length - 1];
        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            texts[i - 1] = elements[1].Trim('\r');
        }

        delayWaitForSeconds = CoroutineUtility.GetWaitForSeconds(delay);

        upperImage.gameObject.SetActive(false);
        underImage.gameObject.SetActive(false);
        upperTextBar.gameObject.SetActive(false);
        underTextBar.gameObject.SetActive(false);
        StartCoroutine(CoStartPrologue());
    }

    private void SkipCurrentDialogue()
    {
        if (isPrologueEnded)
        {
            GoToMainScene();
        }

        if (elapsedTime == 1f)
        {
            isRevealing = false;
        }
        else
        {
            elapsedTime = 1f;
        }
    }

    private void GoToMainScene()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("prologue_complete");
        OnPrologueEnded?.Invoke();
    }

    private IEnumerator CoStartPrologue()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            float ratio = 0f;
            elapsedTime = Time.deltaTime;
            ratio = elapsedTime / imageDuration;
            bool isEven = i % 2 == 0;

            Image image = isEven ? upperImage : underImage;
            Image textBar = isEven ? upperTextBar : underTextBar;
            image.sprite = prologueSprites[i];
            image.gameObject.SetActive(true);
            textBar.gameObject.SetActive(true);
            Color color = image.color;
            Color textbarColor = textBar.color;

            while (ratio < 1f)
            {
                ratio = elapsedTime / imageDuration;
                color.a = ratio;
                textbarColor.a = ratio;
                image.color = color;
                textBar.color = textbarColor;
                elapsedTime += Time.deltaTime;
                elapsedTime = elapsedTime > 1f ? 1f : elapsedTime;
                yield return null;
            }

            TextMeshProUGUI textMeshProUGUI = isEven ? upperText : underText;
            textMeshProUGUI.text = "";
            textMeshProUGUI.gameObject.SetActive(true);
            isRevealing = true;


            foreach (var alphabet in texts[i])
            {
                if (!isRevealing)
                {
                    textMeshProUGUI.text = texts[i];
                    break;
                }

                textMeshProUGUI.text += alphabet;
                yield return delayWaitForSeconds;
            }


            if (i == 1)
            {
                yield return new WaitForSeconds(panelDelay);
                upperText.gameObject.SetActive(false);
                underText.gameObject.SetActive(false);
                upperImage.gameObject.SetActive(false);
                underImage.gameObject.SetActive(false);
            }
        }

        GoToMainScene();
    }
}
