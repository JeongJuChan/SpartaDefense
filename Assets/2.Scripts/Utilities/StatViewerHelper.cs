using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatViewerHelper : MonoBehaviorSingleton<StatViewerHelper>
{
    public Action<int> OnCastleLevelChanged;
    public Action<Sprite> OnUpdateCastleSprite;
    private BattleManager battleManager;
    private Castle castle;

    BigInteger beforeStatData = 0;

    private Dictionary<StatType, Func<BigInteger>> getValueByStatTypeDict;

    public void Init()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        castle = FindAnyObjectByType<Castle>();

        OnUpdateCastleSprite += castle.UpdateSprite;

        // getValueByStatTypeDict = new Dictionary<StatType, Func<BigInteger>>()
        // {
        //     { StatType.Damage, battleManager.heroesStatDataHandler.GetDamage },
        //     { StatType.HP, castle.GetMaxHealth },
        //     { StatType.Defense, castle.GetDefense },
        // };
    }

    /*public BigInteger GetDefaultData(StatType statType)
    {
        switch (statType)
        {
            case StatType.Damage:
                return StatDataHandler.Instance.GetTotalAdditionalPowerUpdated();
            case StatType.HP:
                return castle.GetMaxHealth();
            case StatType.Defense:
                return castle.GetDefense();
        }
        return 0;
    }*/

    public AttributeAppliedData GetAttributeAppliedData()
    {
        return default;
        //return battleManager.attributeDataHandler.GetAttributeAppliedData();
    }

    public BigInteger totalStatData()
    {
        if (!GameManager.instance.isInitializing) return 0;

        BigInteger totalStatData = StatDataHandler.Instance.GetTotalAdditionalPowerUpdated();

        /*float[] attributeStats = GetAttributeAppliedData().attributeAppliedStat;

        if (attributeStats == null)
        {
            return null;
        }

        foreach (var attribute in attributeStats)
        {
            totalStatData += (int)(attribute * 1000);
        }*/

        // beforeStatData = totalStatData;

        return totalStatData;
    }

    public BigInteger GetBattlePowerChange()
    {
        if (!GameManager.instance.isInitializing) return 0;

        if (beforeStatData == 0)
        {
            beforeStatData = totalStatData();
            return 0;
        }

        BigInteger afterStatData = totalStatData();
        BigInteger change = afterStatData - beforeStatData;
        beforeStatData = afterStatData;
        return change;
    }
}
