using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroesStatDataHandler
{
    private Dictionary<ColleagueType, SlotEquipmentStatData> heroesStatDict = new Dictionary<ColleagueType, SlotEquipmentStatData>();
    private Dictionary<ColleagueType, BigInteger> heroDamageDict = new Dictionary<ColleagueType, BigInteger>();

    public event Action<SlotEquipmentStatData> OnAddCastleStat;
    public event Action<SlotEquipmentStatData> OnSubtractCastleStat;

    private AdditionalStatData baseAdditionalStatData;
    private AdditionalStatData rateAdditionalStatData;

    private SlotEquipmentStatData preBaseEquipmentStatData = default;
    private SlotEquipmentStatData preRateEquipmentStatData = default;

    private BigInteger preBaseDamage;

    private BigInteger preRatePercent;

    private Func<Rank, SlotStatData> OnGetSlotStatData;

    private Func<ColleagueType, bool> OnGetIsHeroExist;
    private UI_Alert uI_Alert;

    public HeroesStatDataHandler(BattleManager battleManager, NewSlotUIPanel newSlotUIPanel, Func<Rank, SlotStatData> OnGetSlotStatData)
    {
        /*battleManager.OnGetDamage += GetDamage;
        newSlotUIPanel.OnAddHero += OnHeroStatAdded;
        newSlotUIPanel.OnRemoveHero += OnHeroStatRemoved;
        this.OnGetSlotStatData = OnGetSlotStatData;*/
        StatDataHandler.Instance.OnUpdateTotalAdditionalStat += UpdateAdditionalStat;

        uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();
    }

    private bool GetHeroExist(ColleagueType slotType)
    {
        bool temp = heroesStatDict.ContainsKey(slotType);
        return temp;
    }

    private BigInteger GetDamage(ColleagueType slotType)
    {
        return heroDamageDict[slotType];
    }

    public BigInteger GetDamage()
    {
        //return heroDamageDict[ColleagueType.Rtan];
        // TODO: 수정
        return 1;
    }

    private void UpdateAdditionalStat(ArithmeticStatType arithmeticStatType, AdditionalStatData additionalStatData)
    {
        if (arithmeticStatType == ArithmeticStatType.Base)
        {
            UpdateBaseStats(additionalStatData);
        }
        else if (arithmeticStatType == ArithmeticStatType.Rate)
        {
            UpdateRateStats(additionalStatData);
        }
    }

    /*private void UpdateRateStats()
    {
        CastleProgressionData castleProgressionData = ResourceManager.instance.castleProgressionDataSO.GetCastleProgressionData(ES3.Load(Consts.CASTLE_CURRENT_LEVEL, 1));

        BigInteger damage = castleProgressionData.BaseAttack * rate / Consts.PERCENT_DIVIDE_VALUE;
        BigInteger hp = castleProgressionData.BaseHP * rate / Consts.PERCENT_DIVIDE_VALUE;
        BigInteger defense = castleProgressionData.BaseDefense * rate / Consts.PERCENT_DIVIDE_VALUE;
        SlotEquipmentStatData equipmentRateStatData = new SlotEquipmentStatData(damage, hp, defense, new SlotEquipmentStatDataSave());

        if (preRateEquipmentStatData.health != null)
        {
            OnHeroStatRemoved(SlotType.Rtan, preRateEquipmentStatData);
        }
        OnHeroStatAdded(SlotType.Rtan, equipmentRateStatData);

        preRateEquipmentStatData = equipmentRateStatData;
    }*/

    private void UpdateRateStats(AdditionalStatData additionalStatData)
    {
        rateAdditionalStatData = additionalStatData;
        BigInteger rate = additionalStatData.Stats.ContainsKey(StatType.Damage) ? additionalStatData.Stats[StatType.Damage] : 0;

        BigInteger damage;
        BigInteger hp;
        BigInteger defense;

        CastleProgressionData castleProgressionData = ResourceManager.instance.castleProgressionDataSO.GetCastleProgressionData(ES3.Load(Consts.CASTLE_CURRENT_LEVEL, 1, ES3.settings));
        string[] str = rate.ToString().Split('.');
        if (str.Length == 1)
        {
            damage = castleProgressionData.BaseAttack * rate / Consts.PERCENT_DIVIDE_VALUE;
            hp = castleProgressionData.BaseHP * rate / Consts.PERCENT_DIVIDE_VALUE;
            defense = castleProgressionData.BaseDefense * rate / Consts.PERCENT_DIVIDE_VALUE;
        }
        else
        {
            damage = castleProgressionData.BaseAttack * rate / (Consts.PERCENT_DIVIDE_VALUE * 10);
            hp = castleProgressionData.BaseHP * rate / (Consts.PERCENT_DIVIDE_VALUE * 10);
            defense = castleProgressionData.BaseDefense * rate / (Consts.PERCENT_DIVIDE_VALUE * 10);
        }

        SlotEquipmentStatData equipmentRateStatData = new SlotEquipmentStatData(damage, hp, defense, new SlotEquipmentStatDataSave());

        /*if (preRateEquipmentStatData.health != null)
        {
            OnHeroStatRemoved(ColleagueType.Rtan, preRateEquipmentStatData);
        }
        OnHeroStatAdded(ColleagueType.Rtan, equipmentRateStatData);*/

        preRatePercent = rate;
        preRateEquipmentStatData = equipmentRateStatData;
    }

    private void UpdateRateStats()
    {
        CastleProgressionData castleProgressionData = ResourceManager.instance.castleProgressionDataSO.GetCastleProgressionData(ES3.Load(Consts.CASTLE_CURRENT_LEVEL, 1, ES3.settings));

        BigInteger damage = castleProgressionData.BaseAttack * preRatePercent / Consts.PERCENT_DIVIDE_VALUE;
        BigInteger hp = castleProgressionData.BaseHP * preRatePercent / Consts.PERCENT_DIVIDE_VALUE;
        BigInteger defense = castleProgressionData.BaseDefense * preRatePercent / Consts.PERCENT_DIVIDE_VALUE;
        SlotEquipmentStatData equipmentRateStatData = new SlotEquipmentStatData(damage, hp, defense, new SlotEquipmentStatDataSave());

        /*if (preRateEquipmentStatData.health != null)
        {
            OnHeroStatRemoved(ColleagueType.Rtan, preRateEquipmentStatData);
        }
        OnHeroStatAdded(ColleagueType.Rtan, equipmentRateStatData);*/

        preRateEquipmentStatData = equipmentRateStatData;
    }

    private void UpdateBaseStats(AdditionalStatData additionalStatData)
    {
        baseAdditionalStatData = additionalStatData;
        StatType hpType = StatType.HP;
        BigInteger hpBase = additionalStatData.Stats.ContainsKey(hpType) ? additionalStatData.Stats[hpType] : 0;
        StatType damageType = StatType.Damage;
        BigInteger damageBase = additionalStatData.Stats.ContainsKey(damageType) ? additionalStatData.Stats[damageType] : 0;
        StatType defenseType = StatType.Defense;
        BigInteger defenseBase = additionalStatData.Stats.ContainsKey(defenseType) ? additionalStatData.Stats[defenseType] : 0;
        SlotEquipmentStatData equipmentBaseStatData = new SlotEquipmentStatData(damageBase, hpBase, defenseBase, new SlotEquipmentStatDataSave());

        /*if (preBaseEquipmentStatData.defense != null)
        {
            OnHeroStatRemoved(ColleagueType.Rtan, preBaseEquipmentStatData);
        }

        OnHeroStatAdded(ColleagueType.Rtan, equipmentBaseStatData);*/

        preBaseEquipmentStatData = equipmentBaseStatData;
        CastleProgressionData castleProgressionData = ResourceManager.instance.castleProgressionDataSO.GetCastleProgressionData(ES3.Load(Consts.CASTLE_CURRENT_LEVEL, 1, ES3.settings));
        if (preBaseDamage != castleProgressionData.BaseAttack)
        {
            if (preRatePercent != null)
            {
                UpdateRateStats();
            }

            preBaseDamage = castleProgressionData.BaseAttack;
        }
    }

    private void OnHeroStatAdded(ColleagueType slotType, SlotEquipmentStatData slotStatData)
    {
        if (!heroesStatDict.ContainsKey(slotType))
        {
            heroesStatDict.Add(slotType, slotStatData);
        }

        if (!heroDamageDict.ContainsKey(slotType))
        {
            heroDamageDict.Add(slotType, new BigInteger(0));
        }

        //heroDamageDict[ColleagueType.Rtan] += slotStatData.mainDamage;

        if (slotStatData.health != null || slotStatData.defense != null)
        {
            OnAddCastleStat?.Invoke(slotStatData);
        }

         uI_Alert.PowerMessage(StatViewerHelper.instance.GetBattlePowerChange());
    }

    private void OnHeroStatAdded(ColleagueType slotType, Rank rank, SlotEquipmentStatData slotStatData)
    {
        OnGetSlotStatData.Invoke(rank);

        OnHeroStatAdded(slotType, slotStatData);

        /*if (slotType != ColleagueType.Rtan)
        {
            AddHeroDamageExceptRtan(slotType, rank);
        }*/
        // uI_Alert.PowerMessage(StatViewerHelper.instance.GetBattlePowerChange());
    }

    private void OnHeroStatRemoved(ColleagueType slotType, SlotEquipmentStatData slotStatData)
    {
        if (!heroesStatDict.ContainsKey(slotType))
        {
            heroesStatDict.Add(slotType, slotStatData);
        }

        /*heroDamageDict[ColleagueType.Rtan] -= slotStatData.mainDamage;
        Debug.Log(heroDamageDict[ColleagueType.Rtan]);

        if (slotType != ColleagueType.Rtan)
        {
            ResetHeroDamageExceptRtan(slotType);
        }*/

        if (slotStatData.health != null || slotStatData.defense != null)
        {
            OnSubtractCastleStat?.Invoke(slotStatData);
        }
    }

    private void AddHeroDamageExceptRtan(ColleagueType slotType, Rank rank)
    {
        //heroDamageDict[slotType] = heroDamageDict[ColleagueType.Rtan] * OnGetSlotStatData.Invoke(rank).heroDamagePercent / Consts.PERCENT_DIVIDE_VALUE;
    }

    private void ResetHeroDamageExceptRtan(ColleagueType slotType)
    {
        heroDamageDict[slotType] = 0;
    }
}