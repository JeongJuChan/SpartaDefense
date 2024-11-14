using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class EquipmentManager : MonoBehaviorSingleton<EquipmentManager>
{
    private bool isInitialized = false;

    private EquipmentBaseDataSO[] baseDatas;
    private Dictionary<EquipmentType, EquipmentBaseDataSO> baseDataDic;

    private Dictionary<EquipmentType, List<EquipmentData>> equipmentListOrderedByRank;
    private Dictionary<EquipmentType, List<EquipmentData>> equipmentListsOrderedByEffect;
    private Dictionary<string, EquipmentData> equipmentsDic;
    private Dictionary<string, Sprite> iconsDic;

    private Dictionary<EquipmentType, EquipmentData> equipped;
    private Dictionary<EquipmentType, EquipmentData> recommanded;
    private Dictionary<EquipmentType, bool> compositeAvailabilities;
    private Dictionary<EquipmentType, bool> recommandAvailabilities;

    private Dictionary<EquipmentData, int> compositeResults;

    public event Action<bool> OnRecommandedAvailable;
    public event Action<bool> OnCompositeAllAvailable;

    public event Action OnEquipChange;

    // public List<EquipmentsData> equipmentsData;



    // private void Start()
    // {
    //     Initialize();
    // }

    public void Initialize()
    {
        if (isInitialized) return;

        // equipmentsData = DataParser.ParseEquipmentData(Resources.Load<TextAsset>("CsvData/EquipmentsData"));

        SetReferences();
        SetCollections();
        LoadBaseDatas();
        LoadIcons();
        CreateEquipmentDatas();
        LoadEquippedInfo();


        // QuestManager.instance.GetEventQuestTypeAction.Add(EventQuestType.EquipmentCompositeCount, () => { QuestManager.instance.UpdateCount(EventQuestType.EquipmentCompositeCount, PlayerPrefs.HasKey("EquipmentCompositeCount") ? 1 : 0, -1); });
        // QuestManager.instance.GetEventQuestTypeAction.Add(EventQuestType.EquipmentEquip, () => { QuestManager.instance.UpdateCount(EventQuestType.EquipmentEquip, PlayerPrefs.HasKey("EquipmentEquip") ? 1 : 0, -1); });

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.EquipmentCompositeCount, () => { QuestManager.instance.UpdateCount(EventQuestType.EquipmentCompositeCount, PlayerPrefs.HasKey("EquipmentCompositeCount") ? 1 : 0, -1); });
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.EquipmentEquip, () => { QuestManager.instance.UpdateCount(EventQuestType.EquipmentEquip, PlayerPrefs.HasKey("EquipmentEquip") ? 1 : 0, -1); });


        isInitialized = true;
    }

    public void InitializeChecks()
    {
        AllCheckRecommendAvailability();
        AllCheckCompositeAllAvailability();
    }

    private void SetReferences()
    {
    }

    private void SetCollections()
    {
        baseDataDic = new Dictionary<EquipmentType, EquipmentBaseDataSO>();

        equipmentsDic = new Dictionary<string, EquipmentData>();
        iconsDic = new Dictionary<string, Sprite>();
        equipmentListOrderedByRank = new Dictionary<EquipmentType, List<EquipmentData>>();
        equipmentListsOrderedByEffect = new Dictionary<EquipmentType, List<EquipmentData>>();

        equipped = new Dictionary<EquipmentType, EquipmentData>();
        recommanded = new Dictionary<EquipmentType, EquipmentData>();
        compositeAvailabilities = new Dictionary<EquipmentType, bool>();
        recommandAvailabilities = new Dictionary<EquipmentType, bool>();

        compositeResults = new Dictionary<EquipmentData, int>();
    }

    private void LoadBaseDatas()
    {
        baseDatas = Resources.LoadAll<EquipmentBaseDataSO>("ScriptableObjects/EquipmentBaseDataSO");

        foreach (EquipmentBaseDataSO data in baseDatas)
        {
            // baseDataDic[data.EquipmentType] = data;
            baseDataDic.Add(data.EquipmentType, data);
        }
    }

    private void LoadIcons()
    {
        foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
        {
            Sprite[] icons = Resources.LoadAll<Sprite>($"Sprites/Equipments/{type}");
            foreach (Sprite icon in icons)
            {
                iconsDic[icon.name] = icon;
            }
        }
    }

    private void CreateEquipmentDatas()
    {
        foreach (EquipmentBaseDataSO data in baseDatas)
        {
            List<EquipmentData> list = new List<EquipmentData>();

            foreach (EquipmentRankReference reference in data.RankReferences)
            {
                for (int i = 0; i < reference.equipmentCount; i++)
                {
                    int index = i;
                    EquipmentData equipment = new EquipmentData(data, data.EquipmentType, reference, index);

                    string name = $"{data.EquipmentType}_{reference.rank}_{index}";

                    equipmentsDic[name] = equipment;
                    list.Add(equipment);

                    if (equipment.GetCount() > 0)
                    {
                        var equipmentOwnerValue = GetOwnerEffectValue(equipment);
                        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Equipment, equipment.effectType, equipmentOwnerValue, true);
                    }
                }
            }

            List<EquipmentData> listForSort = new List<EquipmentData>(list);

            equipmentListOrderedByRank[data.EquipmentType] = list;
            equipmentListsOrderedByEffect[data.EquipmentType] = listForSort;

        }
        Debug.Log("Before : " + GetInstanceID());
    }

    private void LoadEquippedInfo()
    {
        foreach (KeyValuePair<EquipmentType, List<EquipmentData>> kvp in equipmentListsOrderedByEffect)
        {
            SortEquipments(kvp.Value);
        }
        AllCheckCompositeAllAvailability();
        AllCheckRecommendAvailability();
    }

    public void Equip(EquipmentData equipment)
    {
        EquipmentType type = equipment.EquipmentType;
        equipped.TryGetValue(equipment.EquipmentType, out EquipmentData prev);

        equipped[type] = equipment;

        if (prev != null)
        {
            prev.UpdateEquippedState(false);
            // RemoveEffect(prev);
        }

        equipment.UpdateEquippedState(true);
        ApplyEffect(equipment, prev);

        OnEquipChange?.Invoke();

        QuestManager.instance.UpdateCount(EventQuestType.EquipmentEquip, 1, -1);

        PlayerPrefs.SetInt("EquipmentEquip", 1);

        CheckRecommendAvailability(type);
    }

    public void RemoveEffect(EquipmentData data)
    {
        var equipmentValue = GetEquipEffectValue(data);
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Equipment, data.effectType, equipmentValue, false);
    }

    public void ApplyEffect(EquipmentData data, EquipmentData prev)
    {
        if (prev == null)
        {
            var equipmentValue = GetEquipEffectValue(data);
            StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Equipment, data.effectType, equipmentValue, true);
        }
        else
        {
            var prevValue = GetEquipEffectValue(prev);
            var equipmentValue = GetEquipEffectValue(data);
            StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Equipment, data.effectType, equipmentValue - prevValue, true);
        }
    }

    public void EquipRecommanded()
    {
        foreach (KeyValuePair<EquipmentType, EquipmentData> kvp in recommanded)
        {
            Equip(kvp.Value);
        }
    }

    public void UpdateEquipmentCount(EquipmentData equipment, int increase)
    {
        equipment.UpdateCount(increase);
    }

    public void AllSort()
    {
        foreach (KeyValuePair<EquipmentType, List<EquipmentData>> kvp in equipmentListsOrderedByEffect)
        {
            SortEquipments(kvp.Value);
        }

        AllCheckCompositeAllAvailability();
        AllCheckRecommendAvailability();
    }

    public void UpdateEquipmentLevel(EquipmentData equipment)
    {
        equipment.LevelUp();

        // questManager.UpdateCount(Enums.EventQuestType.EquipmentEnhance, 1, -1);

        // if (equipment.GetEquippedState()) playerDataManager.UpdateIntData(GetLevelUpUnit(equipment), equipment.EEType, Enums.DataCalculateType.Add);
    }

    public void CompositeEquipment(EquipmentData equipment)
    {
        Composite(equipment);
    }

    public void CompositeAllEquipments()
    {
        compositeResults.Clear();

        foreach (KeyValuePair<EquipmentType, List<EquipmentData>> kvp in equipmentListOrderedByRank)
        {
            List<EquipmentData> list = kvp.Value;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetCount() >= Consts.COMPOSIT_UNIT)
                    Composite(list[i], true);
            }
        }

        AllSort();

        AddCompositeSlot();
        // ui_rewards.ShowUI();
    }

    private void Composite(EquipmentData equipment, bool allComposite = false)
    {
        if (equipment.rank == Rank.Epic && equipment.Index >= 4) return;

        EquipmentRankReference reference = baseDataDic[equipment.EquipmentType].RankReferences[(int)equipment.rank];

        int maxIndex = reference.equipmentCount - 1;
        int index = equipment.Index;
        int targetIndex = (index < maxIndex) ? index + 1 : 0;
        Rank rank = (index < maxIndex) ? equipment.rank : equipment.rank + 1;

        EquipmentData nextEquipment = equipmentsDic[$"{equipment.EquipmentType}_{rank}_{targetIndex}"];

        int count = (equipment.GetCount()) / Consts.COMPOSIT_UNIT;

        if (compositeResults.ContainsKey(equipment)) compositeResults[equipment] -= Consts.COMPOSIT_UNIT * count;
        compositeResults[nextEquipment] = count;

        UpdateEquipmentCount(equipment, -(Consts.COMPOSIT_UNIT * count));
        UpdateEquipmentCount(nextEquipment, count);

        if (!allComposite)
            AllSort();


        nextEquipment.UpdateLevel();

        QuestManager.instance.UpdateCount(EventQuestType.EquipmentCompositeCount, 1, -1);

        PlayerPrefs.SetInt("EquipmentCompositeCount", 1);
    }

    private void AddCompositeSlot()
    {
        // foreach (KeyValuePair<EquipmentData, int> kvp in compositeResults)
        // {
        //     if (kvp.Value > 0) ui_rewards.GetSlot<EquipmentRewardSlot>(Strings.RewardTitle.EQUIPMENT_COMPOSITE_RESULT).SetUI(kvp.Key, kvp.Value);
        // }
    }

    private void CheckRecommendAvailability(EquipmentType type)
    {
        equipped.TryGetValue(type, out EquipmentData equippedEquipment);
        recommanded.TryGetValue(type, out EquipmentData recommandedEquipment);

        recommandAvailabilities[type] = equippedEquipment != recommandedEquipment;

        bool isRecommandAvailable = false;

        foreach (KeyValuePair<EquipmentType, bool> kvp in recommandAvailabilities)
        {
            if (kvp.Value)
            {
                isRecommandAvailable = true;
                break;
            }
        }

        OnRecommandedAvailable?.Invoke(isRecommandAvailable);
    }

    private void AllCheckRecommendAvailability()
    {
        bool isRecommandAvailable = false;
        foreach (var type in Enum.GetValues(typeof(EquipmentType)))
        {
            equipped.TryGetValue((EquipmentType)type, out EquipmentData equippedEquipment);
            recommanded.TryGetValue((EquipmentType)type, out EquipmentData recommandedEquipment);

            recommandAvailabilities[(EquipmentType)type] = equippedEquipment != recommandedEquipment;

            if (recommandAvailabilities[(EquipmentType)type])
            {
                isRecommandAvailable = true;
                break;
            }
        }
        OnRecommandedAvailable?.Invoke(isRecommandAvailable);

    }

    private void CheckCompositeAllAvailability(EquipmentType type)
    {
        List<EquipmentData> list = equipmentListOrderedByRank[type];
        bool isAvailable = false;

        foreach (EquipmentData equipment in list)
        {
            if (equipment.GetCount() >= Consts.COMPOSIT_UNIT)
            {
                isAvailable = true;
                break;
            }
        }

        compositeAvailabilities[type] = isAvailable;

        bool isCompositeAllAvailable = false;

        foreach (KeyValuePair<EquipmentType, bool> kvp in compositeAvailabilities)
        {
            if (kvp.Value)
            {
                isCompositeAllAvailable = true;
                break;
            }
        }

        OnCompositeAllAvailable?.Invoke(isCompositeAllAvailable);
    }

    private void AllCheckCompositeAllAvailability()
    {
        /*bool isCompositeAllAvailable = false;
        foreach (var type in Enum.GetValues(typeof(EquipmentType)))
        {
            List<EquipmentData> list = equipmentListOrderedByRank[(EquipmentType)type];
            bool isAvailable = false;

            foreach (EquipmentData equipment in list)
            {
                if (equipment.GetCount() >= Consts.COMPOSIT_UNIT)
                {
                    isAvailable = true;
                    break;
                }
            }

            compositeAvailabilities[(EquipmentType)type] = isAvailable;

            if (compositeAvailabilities[(EquipmentType)type])
            {
                isCompositeAllAvailable = true;
                break;
            }
        }

        OnCompositeAllAvailable?.Invoke(isCompositeAllAvailable);*/
    }

    public EquipmentBaseDataSO GetBaseData(EquipmentType type)
    {
        return baseDataDic[type];
    }

    public EquipmentData GetData(string name)
    {
        return equipmentsDic[name];
    }

    public Dictionary<string, EquipmentData> GetEquipmentsDic()
    {
        return equipmentsDic;
    }

    public Sprite GetIcon(EquipmentData equipment)
    {
        return iconsDic[equipment.Name];
    }

    public Sprite GetIcon(string name)
    {
        return iconsDic[name];
    }

    public List<EquipmentData> GetDatasByType(EquipmentType type)
    {
        Debug.Log("After : " + GetInstanceID());
        return equipmentListOrderedByRank[type];
    }

    public EquipmentData GetEquippedEquipment(EquipmentType type)
    {
        equipped.TryGetValue(type, out EquipmentData data);

        return data;
    }

    public int GetEquipEffectValue(EquipmentData equipment)
    {
        int value = GetBaseEquipEffectValue(equipment) + equipment.GetLevel() * GetEquipLevelUpUnit(equipment);

        return value;
    }

    public int GetNextEquipEffectValue(EquipmentData equipment)
    {
        int value = GetBaseEquipEffectValue(equipment) + (equipment.GetLevel() + 1) * GetEquipLevelUpUnit(equipment);

        return value;
    }

    public int GetOwnerEffectValue(EquipmentData equipment)
    {
        int value = GetBaseOwnerEffectValue(equipment) + equipment.GetLevel() * GetOnwerLevelUpUnit(equipment);

        return value;
    }

    public int GetEquipmentCountOfRank(EquipmentType type, Rank rank)
    {
        Debug.Log($"어디보자: {type} {rank}");
        // Debug.Log($"어디보자: {baseDataDic[type].RankReferences[(int)rank].rank}");
        return baseDataDic[type].RankReferences[(int)(rank - 1)].equipmentCount;
    }

    public int GetBaseEquipEffectValue(EquipmentData equipment)
    {
        EquipmentRankReference reference = baseDataDic[equipment.EquipmentType].RankReferences[(int)equipment.rank - 1];

        int baseValue = reference.baseEquipEffectStartValue;

        for (int i = 0; i < equipment.Index; i++)
        {
            baseValue = (int)(baseValue * reference.baseEquipEffectIncreaseRate);
        }

        return baseValue;
    }

    public int GetBaseOwnerEffectValue(EquipmentData equipment)
    {
        EquipmentRankReference reference = baseDataDic[equipment.EquipmentType].RankReferences[(int)equipment.rank - 1];

        int baseValue = reference.baseOwnerEffectStartValue;

        for (int i = 0; i < equipment.Index; i++)
        {
            baseValue = (int)(baseValue * reference.baseOwnerEffectIncreaseRate);
        }

        return baseValue;
    }

    public int GetEquipLevelUpUnit(EquipmentData equipment)
    {
        EquipmentRankReference reference = baseDataDic[equipment.EquipmentType].RankReferences[(int)equipment.rank - 1];

        int unit = reference.equipEffectLevelUpUnitStartValue;

        // for (int i = 0; i < equipment.Index - 1; i++)
        // {
        //     unit = (int)(unit * reference.equipEffectLevelUpUnitIncreaseRate);
        // }

        return unit;
    }

    public int GetOnwerLevelUpUnit(EquipmentData equipment)
    {
        EquipmentRankReference reference = baseDataDic[equipment.EquipmentType].RankReferences[(int)equipment.rank - 1];

        int unit = reference.ownerEffectLevelUpUnitStartValue;

        // for (int i = 0; i < equipment.Index - 1; i++)
        // {
        //     unit = (int)(unit * reference.ownerEffectLevelUpUnitIncreaseRate);
        // }

        return unit;
    }
    private void SortEquipments(List<EquipmentData> list)
    {
        list.Sort((a, b) =>
        {
            int countA = a.GetCount();
            int countB = b.GetCount();

            if (countA == 0 && countB == 0)
            {
                return 0;
            }
            else if (countA == 0)
            {
                return 1;
            }
            else if (countB == 0)
            {
                return -1;
            }
            else
            {
                int valueA = GetEquipEffectValue(a);
                int valueB = GetEquipEffectValue(b);

                return valueB.CompareTo(valueA);
            }
        });

        EquipmentType type = list[0].EquipmentType;

        if (list[0].GetCount() > 0) recommanded[type] = list[0];

        CheckRecommendAvailability(type);
    }

    public void ClearCallbacks()
    {
        foreach (KeyValuePair<string, EquipmentData> kvp in equipmentsDic)
        {
            kvp.Value.RemoveAllCallbacks();
        }
    }
}
