using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Playables;

public struct CastleQuestData
{
    public CastleQuestType castleQuestType;
    public string description;
    public int progress;
    public int preProgress;
    public int maxProgress;
    public bool isCompleted;


    private void GetProgress()
    {
        switch (castleQuestType)
        {
            case CastleQuestType.CastleRequiredCharacterLevel:
                progress = ES3.Load<int>(Consts.USER_LEVEL, 1);
                break;
            case CastleQuestType.CastleForgeSlotOpen:
                progress = ES3.Load<int>(Consts.COLLEAGUE_OPEN_SLOT_INDEX, 1);
                progress++;
                break;
            case CastleQuestType.CastleStageClear:
                progress = ES3.Load<int>(Consts.CURRENT_STAGE_INDEX, 1101) >= maxProgress ? 1 : 0;
                break;
        }
    }
    public bool CheckCompleted()
    {
        GetProgress();
        return (progress >= maxProgress) && !LoadCompleted();
    }
    public void SaveCompleted()
    {
        isCompleted = true;
        ES3.Save<bool>($"{castleQuestType}{Consts.COMPLETED_QUEST}", isCompleted, ES3.settings);
        ES3.StoreCachedFile();
    }

    public bool LoadCompleted()
    {
        return isCompleted = ES3.Load<bool>($"{castleQuestType}{Consts.COMPLETED_QUEST}", false);
    }

    public void ResetCompleted()
    {
        isCompleted = false;
        ES3.Save<bool>($"{castleQuestType}{Consts.COMPLETED_QUEST}", isCompleted, ES3.settings);
        ES3.StoreCachedFile();
    }
}
public class CastleProgressionDataHandler
{

    private CastleProgressionDataSO castleProgressionDataSO;
    public CastleProgressionData beforeCastleProgressionData { get; private set; }
    public CastleProgressionData afterCastleProgressionData { get; private set; }
    Dictionary<CastleQuestType, CastleQuestData> castleQuestDatas = new Dictionary<CastleQuestType, CastleQuestData>();

    private int currentLevel = 0;

    public void Init()
    {
        castleProgressionDataSO = ResourceManager.instance.castleProgressionDataSO;

        LoadDatas();

        SetCastleProgressionData(currentLevel);

        StatViewerHelper.instance.OnUpdateCastleSprite?.Invoke(beforeCastleProgressionData.CastleSprite);

        SetCastleQuestDatas();

        // QuestManager.instance.GetEventQuestTypeAction.Add(EventQuestType.CastleUpgrade, () => QuestManager.instance.UpdateCount(EventQuestType.CastleUpgrade, GetCastleLevel(), -1));
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.CastleUpgrade, () => QuestManager.instance.UpdateCount(EventQuestType.CastleUpgrade, GetCastleLevel(), -1));

        InitialStats();

        // QuestManager.instance.GetQuestTypeAction.Add(QuestType.PlayerLevel, () => CanUpgradeCastle());
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.PlayerLevel, () => CanCaslteReddot());
        // QuestManager.instance.GetQuestTypeAction.Add(QuestType.StageClear, () => CanUpgradeCastle());
        QuestManager.instance.AddQuestTypeAction(QuestType.StageClear, () => CanCaslteReddot());
        // QuestManager.instance.GetQuestTypeAction.Add(QuestType., () => CanUpgradeCastle());

    }

    private void InitialStats()
    {
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Castle, StatType.Damage, beforeCastleProgressionData.BaseAttack, true);
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Castle, StatType.HP, beforeCastleProgressionData.BaseHP, true);
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Castle, StatType.Defense, beforeCastleProgressionData.BaseDefense, true);
    }

    private int GetCastleLevel()
    {
        return currentLevel;
    }

    public bool CheckCastleRedDot()
    {
        if (castleQuestDatas.Count <= 0) return false;
        return castleQuestDatas[CastleQuestType.CastleStageClear].CheckCompleted() || castleQuestDatas[CastleQuestType.CastleRequiredCharacterLevel].CheckCompleted() || castleQuestDatas[CastleQuestType.CastleForgeSlotOpen].CheckCompleted();
    }

    public void UpgradeCastle()
    {
        ApplyCastleStat();

        currentLevel++;

        SetCastleProgressionData(currentLevel);
        SetCastleQuestDatas();
        foreach (CastleQuestData castleQuestData in castleQuestDatas.Values)
        {
            castleQuestData.ResetCompleted();
        }

        QuestManager.instance.UpdateCount(EventQuestType.CastleUpgrade, GetCastleLevel(), -1);

        StatViewerHelper.instance.OnCastleLevelChanged?.Invoke(currentLevel);
        StatViewerHelper.instance.OnUpdateCastleSprite?.Invoke(beforeCastleProgressionData.CastleSprite);

        NotificationManager.instance.SetNotification(RedDotIDType.CastleUpgradeButton.ToString(), false);

        SaveDatas();
    }

    public bool GetIsUpgradePossible()
    {
        if (currentLevel < castleProgressionDataSO.GetCastleMaxLevel())
        {
            return true;
        }

        return false;
    }

    public CastleQuestData GetCastleQuestData(CastleQuestType castleQuestType)
    {
        return castleQuestDatas[castleQuestType];
    }

    public bool CanUpgradeCastle()
    {
        var canUpgeade = castleQuestDatas[CastleQuestType.CastleStageClear].LoadCompleted() && castleQuestDatas[CastleQuestType.CastleRequiredCharacterLevel].LoadCompleted() && castleQuestDatas[CastleQuestType.CastleForgeSlotOpen].LoadCompleted();
        NotificationManager.instance.SetNotification(RedDotIDType.CastleUpgradeButton.ToString(), canUpgeade);
        return canUpgeade;
    }

    public bool CanCaslteReddot()
    {
        if (castleQuestDatas.Count <= 0) return false;

        var canCastleReddot = castleQuestDatas[CastleQuestType.CastleStageClear].CheckCompleted() || castleQuestDatas[CastleQuestType.CastleRequiredCharacterLevel].CheckCompleted() || castleQuestDatas[CastleQuestType.CastleForgeSlotOpen].CheckCompleted() || CanUpgradeCastle();
        NotificationManager.instance.SetNotification(RedDotIDType.ShowCastleButton.ToString(), canCastleReddot);
        return canCastleReddot;
    }

    private void SetCastleProgressionData(int level)
    {
        beforeCastleProgressionData = castleProgressionDataSO.GetCastleProgressionData(level);
        afterCastleProgressionData = castleProgressionDataSO.GetCastleProgressionData(level + 1);
    }

    public CastleProgressionData GetCurrentCastleProgressionData()
    {
        return beforeCastleProgressionData;
    }

    public bool SetCastleQuestDatas()
    {
        castleQuestDatas.Clear();

        if (currentLevel >= castleProgressionDataSO.GetCastleMaxLevel())
        {
            return true;
        }

        CastleQuestData castleQuestData = new CastleQuestData();
        castleQuestData.castleQuestType = CastleQuestType.CastleRequiredCharacterLevel;
        castleQuestData.description = $"캐릭터 레벨 Lv.{beforeCastleProgressionData.RequiredCharacterLevel}\n 달성";
        castleQuestData.progress = ES3.Load<int>(Consts.USER_LEVEL, 1);
        castleQuestData.maxProgress = beforeCastleProgressionData.RequiredCharacterLevel;
        castleQuestDatas.Add(CastleQuestType.CastleRequiredCharacterLevel, castleQuestData);
        castleQuestData.LoadCompleted();

        castleQuestData = new CastleQuestData();
        castleQuestData.castleQuestType = CastleQuestType.CastleForgeSlotOpen;
        string slotTypeKR = EnumToKRManager.instance.GetEnumToKR((ColleagueType)beforeCastleProgressionData.ForgeSlotOpen);
        castleQuestData.description = $"{beforeCastleProgressionData.ForgeSlotOpen}번 슬롯 오픈";
        //castleQuestData.progress = ES3.Load<int>(Consts.FORGE_SLOT_COUNT, 1);
        castleQuestData.maxProgress = beforeCastleProgressionData.ForgeSlotOpen;
        castleQuestDatas.Add(CastleQuestType.CastleForgeSlotOpen, castleQuestData);
        castleQuestData.LoadCompleted();

        castleQuestData = new CastleQuestData();
        castleQuestData.castleQuestType = CastleQuestType.CastleStageClear;
        castleQuestData.description = $"{Difficulty.TransformStageNumber(beforeCastleProgressionData.stageClear)}\n 완료";
        castleQuestData.progress = ES3.Load<int>(Consts.CURRENT_STAGE_INDEX, 1101);
        castleQuestData.maxProgress = beforeCastleProgressionData.stageClear;
        castleQuestDatas.Add(CastleQuestType.CastleStageClear, castleQuestData);
        castleQuestData.LoadCompleted();


        foreach (CastleQuestData data in castleQuestDatas.Values)
        {
            if ((data.progress >= data.maxProgress) && !data.LoadCompleted())
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ShowCastleButton.ToString(), true);
                break;
            }
        }

        return false;
    }

    private void ApplyCastleStat()
    {
        var value = new List<KeyValuePair<StatType, BigInteger>>
        {
             new KeyValuePair<StatType, BigInteger>(StatType.Damage, afterCastleProgressionData.BaseAttack - beforeCastleProgressionData.BaseAttack)
                , new KeyValuePair<StatType, BigInteger>(StatType.HP, afterCastleProgressionData.BaseHP - beforeCastleProgressionData.BaseHP)
                , new KeyValuePair<StatType, BigInteger>(StatType.Defense, afterCastleProgressionData.BaseDefense - beforeCastleProgressionData.BaseDefense)
             };
        StatDataHandler.Instance.ModifyStats(ArithmeticStatType.Base, AdditionalStatType.Castle, value, true, true);
    }


    public void SaveDatas()
    {
        ES3.Save<int>(Consts.CASTLE_CURRENT_LEVEL, currentLevel, ES3.settings);

        ES3.StoreCachedFile();
    }

    public void LoadDatas()
    {
        currentLevel = ES3.Load<int>(Consts.CASTLE_CURRENT_LEVEL, 1, ES3.settings);

        StatViewerHelper.instance.OnCastleLevelChanged?.Invoke(currentLevel);

        foreach (CastleQuestData castleQuestData in castleQuestDatas.Values)
        {
            castleQuestData.LoadCompleted();
        }
    }
}