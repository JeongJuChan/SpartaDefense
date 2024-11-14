using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongTouchButton : Button
{
    private float elapsedTime;

    private float touchDuration;
    private bool isTouching;

    private float clickInterval;
    private float clickIntervalElapsedDuration;
    private float clickMinimumInterval;
    private float clickDeacreasingInterval;

    private void Update()
    {
        if (isTouching)
        {
            TryIncreaseButtonClickedCount();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isTouching = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        ResetSettings();
    }

    private void DecreaseInterval()
    {
        clickIntervalElapsedDuration = 0;

        if (clickDeacreasingInterval > clickMinimumInterval)
        {
            clickDeacreasingInterval -= Time.deltaTime;
            clickDeacreasingInterval = clickDeacreasingInterval < clickMinimumInterval ? 
                clickMinimumInterval : clickDeacreasingInterval;
        }
    }

    private void ResetSettings()
    {
        clickDeacreasingInterval = clickInterval;
        clickIntervalElapsedDuration = 0f;
        elapsedTime = 0f;
        isTouching = false;
    }

    public void SetTouchDuration(float touchDuration)
    {
        this.touchDuration = touchDuration;
        onClick.AddListener(DecreaseInterval);
    }

    public void SetClickInterval(float clickInterval, float clickMinimumInterval)
    {
        this.clickInterval = clickInterval;
        clickDeacreasingInterval = clickInterval;
        this.clickMinimumInterval = clickMinimumInterval;
    }

    private void TryIncreaseButtonClickedCount()
    {
        if (elapsedTime >= touchDuration)
        {
            clickIntervalElapsedDuration += Time.deltaTime;
            if (clickIntervalElapsedDuration >= clickDeacreasingInterval)
            {
                onClick.Invoke();
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
