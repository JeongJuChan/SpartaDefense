using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RewardManager : MonoBehaviorSingleton<RewardManager>
{
    private RewardResultPanel rewardResultPanel;
    private RewardInfoProvider rewardInfoProvider;
    public Func<int, int, int, int> offlineRewardCallback;

    public void Initialize()
    {
        rewardInfoProvider = new RewardInfoProvider();

        rewardResultPanel = UIManager.instance.GetUIElement<RewardResultPanel>();

        rewardResultPanel.Initalize();

    }

    // public void AddReward(RewardType type, int amount)
    // {
    //     rewards.Add((rewardInfoProvider.GetRewardInfo(type), amount));
    // }

    public IRewardInfo GetRewardInfo(RewardType type)
    {
        return rewardInfoProvider.GetRewardInfo(type);
    }

    public RewardSlot GetRewardSlot(RewardType type, int amount)
    {
        return rewardResultPanel.GetRewardSlot(type, amount, rewardInfoProvider.GetRewardInfo(type));
    }

    public void GiveReward(RewardType type, int amount)
    {
        if (rewardInfoProvider.GetRewardInfo(type) == null) return;
        IRewardInfo rewardInfo = rewardInfoProvider.GetRewardInfo(type);
        RewardSlot slot = rewardResultPanel.GetRewardSlot(type, amount, rewardInfo);
        slot.SetUI(rewardInfo, amount);
    }

    public void GiveReward(RewardType type, BigInteger amount)
    {
        if (rewardInfoProvider.GetRewardInfo(type) == null) return;
        IRewardInfo rewardInfo = rewardInfoProvider.GetRewardInfo(type);
        RewardSlot slot = rewardResultPanel.GetRewardSlot(type, amount, rewardInfo);
        slot.SetUI(rewardInfo, amount);
    }

    public Sprite GetIcon(RewardType type)
    {
        return rewardInfoProvider.GetRewardInfo(type).GetIcon();
    }

    public void ShowRewardPanel()
    {
        rewardResultPanel.ShowRewardSlot();
    }

    public void ShowStageClearRewardPanel()
    {
        rewardResultPanel.ShowStageClearRewardSlot();
    }
}

