using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumUtility
{
    public static T GetEqualValue<T>(string element)
    {
        var values = Enum.GetValues(typeof(T));
        foreach (T tempValue in values)
        {
            if (tempValue.ToString() == element)
            {
                return tempValue;
            }
        }

        return default;
    }

    public static CurrencyType ChangeRewardCurrency(RewardType type)
    {
        switch (type)
        {
            case RewardType.Gold:
                return CurrencyType.Gold;
            case RewardType.Gem:
                return CurrencyType.Gem;
            case RewardType.ForgeTicket:
                return CurrencyType.ForgeTicket;
            case RewardType.ColleagueLevelUpStone:
                return CurrencyType.ColleagueLevelUpStone;
            case RewardType.AccelerationTicket:
                return CurrencyType.AccelerationTicket;
            case RewardType.ForgeDungeonTicket:
                return CurrencyType.ForgeDungeonTicket;
            case RewardType.GemDungeonTicket:
                return CurrencyType.GemDungeonTicket;
            case RewardType.GoldDungeonTicket:
                return CurrencyType.GoldDungeonTicket;
            case RewardType.ColleagueLevelUpDungeonTicket:
                return CurrencyType.ColleagueLevelUpDungeonTicket;
            case RewardType.AbilityPoint:
                return CurrencyType.AbilityPoint;
            case RewardType.ColleagueSummonTicket:
                return CurrencyType.ColleagueSummonTicket;
            default:
                return CurrencyType.None;
        }
    }

    public static CurrencyType GetCurrencyTypeByDungeonType(DungeonType dungeonType)
    {
        switch (dungeonType)
        {
            case DungeonType.GoldDungeon:
                return CurrencyType.GoldDungeonTicket;
            case DungeonType.GemDungeon:
                return CurrencyType.GemDungeonTicket;
            case DungeonType.ForgeTicketDungeon:
                return CurrencyType.ForgeDungeonTicket;
            case DungeonType.ColleagueLevelUpStoneDungeon:
                return CurrencyType.ColleagueLevelUpDungeonTicket;
            default:
                Debug.Assert(dungeonType != DungeonType.None, "DungeonType이 None입니다.");
                return CurrencyType.None;
        }
    }

    public static StatType? ConvertToStatType(AbilityOptionEffectType? effectType)
    {
        switch (effectType)
        {
            case AbilityOptionEffectType.Damage:
                return StatType.Damage;
            case AbilityOptionEffectType.Defense:
                return StatType.Defense;
            case AbilityOptionEffectType.HP:
                return StatType.HP;
            default:
                return null;
        }
    }

    public static AttributeType? ConvertToAttributeType(AbilityOptionEffectType? effectType)
    {
        switch (effectType)
        {
            case AbilityOptionEffectType.AttackSpeedRate:
                return AttributeType.AttackSpeedRate;
            case AbilityOptionEffectType.CirticalProbability:
                return AttributeType.CriticalProbability;
            case AbilityOptionEffectType.CriticalMultiplication:
                return AttributeType.CriticalMultiplication;
            case AbilityOptionEffectType.Heal:
                return AttributeType.Heal;
            default:
                return null;
        }
    }
}
