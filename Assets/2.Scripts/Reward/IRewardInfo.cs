using UnityEngine;

public interface IRewardInfo
{
    Sprite GetIcon();
    Color GetBackground();
}

public class GoldRewardInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.Gold).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Common);
}

public class ExpRewardInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.Exp).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Rare);
}

public class GemRewardInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.Gem).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class ForgeTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.ForgeTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class ColleagueLevelUpStoneInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.ColleagueLevelUpStone).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class ForgeDungeonTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.ForgeDungeonTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class GemDungeonTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.GemDungeonTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class GoldDungeonTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.GoldDungeonTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class ColleagueLevelUpDungeonTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.ColleagueLevelUpDungeonTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class AccelerationTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.AccelerationTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class AbilityPointInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.AbilityPoint).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class EquipmentRewardInfo : IRewardInfo
{
    private EquipmentType type;
    private Rank rank;
    private int index;
    public string name { get; private set; }

    public EquipmentRewardInfo(EquipmentType type, Rank rank, int index)
    {
        this.type = type;
        this.rank = rank;
        this.index = index;

        name = $"{type}_{rank}_{index}";
    }

    public Sprite GetIcon() => Resources.Load<Sprite>($"Sprites/Equipments/{type}/{name}");
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(rank);
}

public class SkillRewardInfo : IRewardInfo
{
    private Rank rank;
    public int index { get; private set; }

    public SkillRewardInfo(Rank rank, int index)
    {
        this.rank = rank;
        this.index = index;
    }

    public Sprite GetIcon() => Resources.Load<Sprite>($"Sprites/Skills/{rank}_{index}");
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(rank);
}

public class ColleagueSummonTicketInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.ColleagueSummonTicket).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Epic);
}

public class ColleagueRewardInfo : IRewardInfo
{
    private ColleagueType colleagueType;
    private Rank rank;
    private int index;

    public ColleagueRewardInfo(ColleagueType colleagueType, Rank rank, int index)
    {
        this.colleagueType = colleagueType;
        this.rank = rank;
        this.index = index;
    }

    public Sprite GetIcon() => ResourceManager.instance.slotHeroData.GetResource(new ColleagueInfo(rank, colleagueType)).defaultSprite;
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(rank);
    public int GetIndex() => index;
}