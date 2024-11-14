using System;

public class ForgeLevelDataHandler
{
    private ForgeLevelData forgeLevelData;

    private Action OnUpdateForgeLevelUI;

    public ForgeLevelDataHandler(SlotEquipmentForger slotEquipmentForger, ForgeProbabilityUIPanel forgeProbabilityUIPanel,
        Func<int, CastleDoorRankProbabilityData> getCastleDoorRankProbabilityData, Func<int> getCastleDataCount, AutoForgeUIPanel autoForgeUIPanel)
    {
        //TODO: 여기야
        // 레벨, 경험치 불러오기
        forgeLevelData = new ForgeLevelData(1, 0, 2);

        LoadDatas();

        slotEquipmentForger.OnGetCastleLevel += GetCastleLevel;
        autoForgeUIPanel.OnGetForgeLevel += GetCastleLevel;
        forgeProbabilityUIPanel.OnGetForgeLevelData += GetCastleLevelData;
        forgeProbabilityUIPanel.OnTryLevelUp += TryLevelUp;
        forgeProbabilityUIPanel.OnGetCastleDoorRankProbabilityData += getCastleDoorRankProbabilityData;
        forgeProbabilityUIPanel.OnGetCastleDoorRankProbabilityCount += getCastleDataCount;
        forgeProbabilityUIPanel.OnAddExp += AddCurrentExp;
        OnUpdateForgeLevelUI += forgeProbabilityUIPanel.UpdateLevelText;

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.ForgeLevelAchieve, () => QuestManager.instance.UpdateCount(EventQuestType.ForgeLevelAchieve, GetCastleLevel(), -1));
    }

    private int GetCastleLevel()
    {
        return forgeLevelData.forgeLevel;
    }

    public ForgeLevelData GetCastleLevelData()
    {
        return forgeLevelData;
    }

    private void TryLevelUp()
    {
        if (forgeLevelData.currentExp >= forgeLevelData.maxExp)
        {
            forgeLevelData.forgeLevel++;
            forgeLevelData.currentExp -= forgeLevelData.maxExp;
            QuestManager.instance.UpdateCount(EventQuestType.ForgeLevelAchieve, GetCastleLevel(), -1);
            UpdateMaxLevel();
            SaveData_ForgeLevel();
            OnUpdateForgeLevelUI?.Invoke();
        }
    }

    private void AddCurrentExp()
    {
        forgeLevelData.currentExp++;

        SaveData_CurrentExp();
    }

    private void UpdateMaxLevel()
    {
        forgeLevelData.maxExp = forgeLevelData.forgeLevel + 1;
    }

    private void SaveData_ForgeLevel()
    {
        ES3.Save<int>(Consts.FORGE_LEVEL, forgeLevelData.forgeLevel, ES3.settings);
        ES3.StoreCachedFile();
    }
    private void SaveData_CurrentExp()
    {
        ES3.Save<int>(Consts.FORGE_CURRENT_EXP, forgeLevelData.currentExp, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void SaveDatas()
    {
        SaveData_ForgeLevel();
        SaveData_CurrentExp();
    }

    private void LoadDatas()
    {
        // if (!ES3.KeyExists("CastleLevel")) return;

        forgeLevelData.forgeLevel = ES3.Load<int>(Consts.FORGE_LEVEL, 1);
        forgeLevelData.currentExp = ES3.Load<int>(Consts.FORGE_CURRENT_EXP, 0);
        forgeLevelData.maxExp = forgeLevelData.forgeLevel + 1;
    }
}