using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardInfoProvider
{
    private Dictionary<RewardType, IRewardInfo> rewardInfos;

    public RewardInfoProvider()
    {
        rewardInfos = new Dictionary<RewardType, IRewardInfo>
        {
            { RewardType.Gold, new GoldRewardInfo() },
            { RewardType.Exp, new ExpRewardInfo() },
            { RewardType.Gem, new GemRewardInfo() },
            { RewardType.ColleagueLevelUpStone, new ColleagueLevelUpStoneInfo() },
            { RewardType.ColleagueLevelUpDungeonTicket, new ColleagueLevelUpDungeonTicketInfo() },
            { RewardType.GemDungeonTicket, new GemDungeonTicketInfo() },
            { RewardType.GoldDungeonTicket, new GoldDungeonTicketInfo() },
            { RewardType.ForgeDungeonTicket, new ForgeDungeonTicketInfo() },
            { RewardType.ForgeTicket, new ForgeTicketInfo() },
            { RewardType.Equipment, new EquipmentRewardInfo(EquipmentType.Bow, Rank.Common, 1) },
            { RewardType.AccelerationTicket, new AccelerationTicketInfo() },
            { RewardType.AbilityPoint, new AbilityPointInfo() },
            { RewardType.ColleagueSummonTicket, new ColleagueSummonTicketInfo() },
            { RewardType.Colleague_Warrior_21, new ColleagueRewardInfo(ColleagueType.Warrior_Common, Rank.Common, 1) },
            // Add other rewards similarly
        };
    }

    public IRewardInfo GetRewardInfo(RewardType type)
    {
        if (!rewardInfos.ContainsKey(type)) return null;
        return rewardInfos[type];
    }
}

