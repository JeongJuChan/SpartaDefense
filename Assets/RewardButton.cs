using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : RewardSlot
{
    [SerializeField] Image clearImage;
    [SerializeField] Image rewardGlow;
    [SerializeField] Image redDot;
    [SerializeField] public Button rewardButton;
    public bool isRewarded { get; private set; } = false;


    public void Init()
    {
        isRewarded = ES3.Load<bool>($"RewardButton_{type}_{amount}", false);

        if (isRewarded)
        {
            rewardGlow.gameObject.SetActive(false);
            redDot.enabled = false;
            rewardButton.interactable = false;
            clearImage.gameObject.SetActive(true);
        }
    }

    public void SetComplete()
    {
        if (isRewarded)
        {
            GetReward();
            return;
        }
        rewardGlow.gameObject.SetActive(true);
        redDot.enabled = true;
        rewardButton.interactable = true;
    }

    public void GetReward()
    {
        rewardGlow.gameObject.SetActive(false);
        redDot.enabled = false;
        rewardButton.interactable = false;
        clearImage.gameObject.SetActive(true);
        isRewarded = true;

        ES3.Save<bool>($"RewardButton_{type}_{amount}", true, ES3.settings);
    }

    public void Reset()
    {
        isRewarded = false;
        rewardGlow.gameObject.SetActive(false);
        redDot.enabled = false;
        rewardButton.interactable = false;
        clearImage.gameObject.SetActive(false);

        ES3.Save<bool>($"RewardButton_{type}_{amount}", false, ES3.settings);
    }

}
