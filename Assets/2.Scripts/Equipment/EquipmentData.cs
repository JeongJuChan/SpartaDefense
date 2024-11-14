using System;
using UnityEngine;

public class EquipmentData : ISummonable
{
    private class EquipmentDataSave
    {
        public int count;
        public bool isEquipped;

        public EquipmentDataSave(int count, bool isEquipped)
        {
            this.count = count;
            this.isEquipped = isEquipped;
        }
    }

    private EquipmentBaseDataSO baseData;
    private EquipmentRankReference rankReference;

    public string Name { get; private set; }

    public EquipmentType EquipmentType { get; private set; }
    public StatType effectType { get; private set; }

    public int Index { get; private set; }
    public Rank rank { get; private set; }

    // 강화할 데이터

    private int count;
    private int currentLevel;
    private bool isEquipped;

    public event Action OnCountChange;
    public event Action OnLevelChange;
    public event Action OnEquippedChange;

    public event Action UpdateLevelEvent;


    public EquipmentData(EquipmentBaseDataSO baseData, EquipmentType equipmentType, EquipmentRankReference rankReference, int index)
    {
        this.baseData = baseData;
        this.rankReference = rankReference;

        EquipmentType = equipmentType;

        Index = index;
        rank = rankReference.rank;

        Name = $"{equipmentType}_{rank}_{Index}";

        effectType = baseData.EffectType;

        LoadMutableDatas();
    }

    public void UpdateCount(int increase)
    {
        count += increase;
        OnCountChange?.Invoke();

        if (!ES3.KeyExists($"{Consts.EQUIPMENT_ACTIVE_NAME}{Name}"))
        {
            currentLevel = 1;


            ES3.Save($"{Consts.EQUIPMENT_ACTIVE_NAME}{Name}", true, ES3.settings);

            ES3.StoreCachedFile();

            SaveData();

            EncyclopediaDataHandler.Instance.SlotLevelChangeEvent(EncyclopediaType.Equipment, $"{EquipmentType}_{rank}");

            return;
        }

#if UNITY_EDITOR
        Debug.Assert(count >= 0, "Total count of the equipment is less than 0.");
#endif

        SaveData();
    }

    public void UpdateEquippedState(bool isEquipped)
    {
        this.isEquipped = isEquipped;
        OnEquippedChange?.Invoke();

        SaveData();
    }

    public void LevelUp()
    {
        if (IsAtMaxLevel()) return;

        currentLevel++;
        OnLevelChange?.Invoke();

        SaveData();
    }

    public void UpdateLevel()
    {
        UpdateLevelEvent?.Invoke();
        SaveData();
    }

    public int GetCount()
    {
        return count;
    }

    public int GetLevel()
    {
        return currentLevel;
    }

    public bool GetEquippedState()
    {
        return isEquipped;
    }

    public bool IsAtMaxLevel()
    {
        int maxLevel = rankReference.maxLevel;

        if (currentLevel == maxLevel) return true;
        else return false;
    }

    public void RemoveAllCallbacks()
    {
        OnCountChange = null;
        OnLevelChange = null;
        OnEquippedChange = null;
    }

    private void LoadMutableDatas()
    {
        if (ES3.KeyExists($"{Name}{Consts.EQUIPMENT_DATA}"))
        {
            EquipmentDataSave load = ES3.Load<EquipmentDataSave>($"{Name}{Consts.EQUIPMENT_DATA}");
            currentLevel = ES3.Load<int>($"{Name}_Level");
                        
            count = load.count;
            isEquipped = load.isEquipped;
        }
        else
        {
            count = 0;
            currentLevel = 0;
            isEquipped = false;
        }

        if (isEquipped) EquipmentManager.instance.Equip(this);

        SaveData();
    }

    private void SaveData()
    {
        EquipmentDataSave save = new EquipmentDataSave(count, isEquipped);
        ES3.Save($"{Name}{Consts.EQUIPMENT_DATA}", save, ES3.settings);
        ES3.Save($"{Name}_Level", currentLevel, ES3.settings);
        
        ES3.StoreCachedFile();
    }
}
