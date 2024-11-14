using Keiwando.BigInteger;
using System;
using UnityEngine;

public class UserLevelDataHandler
{
    public event Action<int, BigInteger, BigInteger> OnUpdateExp;
    public event Action OnUpdateCastleQuest;

    private Action<int> OnUpdateForgeLevel;

    private int level = 1;
    private BigInteger exp = 0;
    private BigInteger maxExp;

    private UserLevelData userLevelCSVData;

    public Action<FeatureType> OnUpdateFeature;

    public UserLevelDataHandler(UserLevelData userLevelData)
    {
        userLevelCSVData = userLevelData;

        LoadDatas();
        maxExp = userLevelCSVData.baseExp + (level - 1) * userLevelCSVData.increment;

        ForgeManager.instance.OnIncreaseExp += UpdateExp;

        ForgeManager.instance.OnGetUserLevel += GetUserLevel;

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.PlayerLevel, () => { QuestManager.instance.UpdateCount(EventQuestType.PlayerLevel, GetUserLevel(), -1); });

        UnlockManager.Instance.SetUnlockCondition(FeatureType.Level, CheckCurrentLevel);
    }

    public void StartInit()
    {
        OnUpdateFeature += UnlockManager.Instance.CheckUnlocks;
        OnUpdateForgeLevel += UIManager.instance.GetUIElement<UI_Growth>().UpdateForgeLevel;
        OnUpdateFeature?.Invoke(FeatureType.Level);
        OnUpdateForgeLevel?.Invoke(level);
    }

    private bool CheckCurrentLevel(int level)
    {
        return this.level >= level;
    }

    public void UpdateExp(BigInteger expAmount)
    {
        exp += expAmount;
        if (exp >= maxExp)
        {
            exp -= maxExp;
            level++;

            OnUpdateForgeLevel?.Invoke(level);

            maxExp = userLevelCSVData.baseExp + (level - 1) * userLevelCSVData.increment;
            QuestManager.instance.UpdateCount(EventQuestType.PlayerLevel, level, -1);
            OnUpdateFeature?.Invoke(FeatureType.Level);
        }

        OnUpdateExp?.Invoke(level, exp, maxExp);

        SaveDatas();
    }

    private int GetUserLevel()
    {
        return level;
    }

    private void SaveDatas()
    {
        ES3.Save<int>(Consts.USER_LEVEL, level, ES3.settings);
        ES3.Save<string>(Consts.USER_EXP, exp.ToString(), ES3.settings);

        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        if (!ES3.KeyExists(Consts.USER_LEVEL)) return;

        level = ES3.Load<int>(Consts.USER_LEVEL);
        exp = new BigInteger(ES3.Load<string>(Consts.USER_EXP));
    }
}