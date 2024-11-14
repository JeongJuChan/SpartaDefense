using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgePreImagePanel : UI_Base
{
    [SerializeField] private Button preImageButton;
    [SerializeField] private Image effectIamge;
    [SerializeField] private GameObject preImageScaleParent;
    [SerializeField] private Transform originTransform;

    [SerializeField] private Button disableButton;

    private Vector2 targetPos;

    public event Func<float> OnGetSlotElementSize;

    public event Action OnClickDisableButton;
    public event Action OnClickPreImageButton;


    private float elementSize;

    private float showingPopupDuration;
    private float showingSlotImageWaitPercent;

    private float showingSubtractPercent;

    public void Init(float showingPopupDuration, float showingSlotImageWaitPercent)
    {
        this.showingPopupDuration = showingPopupDuration;

        this.showingSlotImageWaitPercent = showingSlotImageWaitPercent;

        showingSubtractPercent = 1f - showingSlotImageWaitPercent;

        targetPos = effectIamge.transform.localPosition;

        disableButton.onClick.AddListener(ClickDisableButton);

        preImageButton.onClick.AddListener(() => OnClickPreImageButton.Invoke());
        ActivateSelf(false);
    }

    private void ActivateSelf(bool isActive)
    {
        effectIamge.gameObject.SetActive(isActive);
        preImageScaleParent.SetActive(isActive);
    }

    private void ClickDisableButton()
    {
        OnClickDisableButton?.Invoke();
    }

    public void ActivatePreImageButton(bool isActive)
    {
        preImageButton.enabled = isActive;
    }

    public void UpdatePreImage(Color color, Sprite sprite)
    {
        effectIamge.color = color;

        preImageButton.image.sprite = sprite;

        disableButton.gameObject.SetActive(true);
        ActivatePreImageButton(false);

        StartCoroutine(CoWaitForAppear());
    }

    public void UpdatePreImageRightAway(Color color, Sprite sprite)
    {
        effectIamge.color = color;
        preImageButton.image.sprite = sprite;
        disableButton.gameObject.SetActive(false);
        ActivatePreImageButton(true);
        ActivateSelf(true);
        effectIamge.transform.localPosition = targetPos;
    }

    private IEnumerator CoWaitForAppear()
    {
        float elapsedTime = Time.deltaTime;
        float ratio = 0f;

        ActivateSelf(true);

        Color color = effectIamge.color;
        Color preImageColor = preImageButton.image.color;
        preImageColor.a = 0f;
        preImageButton.image.color = preImageColor;

        effectIamge.transform.localPosition = originTransform.localPosition;

        while (ratio < 1f)
        {
            ratio = elapsedTime / showingPopupDuration;
            color.a = ratio;

            color.a = Mathf.Lerp(color.a, 1, ratio);
            effectIamge.color = color;

            if (ratio >= showingSlotImageWaitPercent)
            {
                preImageColor.a = (ratio - showingSlotImageWaitPercent) / showingSubtractPercent;
                preImageButton.image.color = preImageColor;
            }

            if (ratio <= showingSlotImageWaitPercent)
            {
                Vector2 currentPos = Vector2.Lerp(effectIamge.transform.localPosition, targetPos, ratio / showingSlotImageWaitPercent);
                effectIamge.transform.localPosition = currentPos;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        disableButton.gameObject.SetActive(false);
    }

    public void OnSlotSold()
    {
        ActivateSelf(false);
    }
}
