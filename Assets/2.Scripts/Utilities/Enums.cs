using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rank
{
    None,
    Common,
    Magic,
    Elaboration,
    Rare,
    Excellent,
    Epic,
}

public enum SummonType
{
    None,
    Equipment,
    Skill,
    Colleague,
}

public enum ColleagueType
{
    None,
    Rtan_Common,
    Rtan_Magic,
    Rtan_Elaboration,
    Rtan_Rare,
    Warrior_Common,
    Mage_Common,
    Rogue_Common,
    Archer_Magic,
    Paladin_Magic,
    Hunter_Magic,
    Shaman_Elaboration,
    Priest_Elaboration,
    Summoner_Elaboration,
    Knight_Rare,
    Assassin_Rare,
}

public enum ColleagueEncyclopediaType
{
    None,
    RtanWarrior,
    MageRogue,
    ArcherPaladin,
    RogueHunter,
    ShamanPriestSummoner,
    KnightArcher,
    AssassinPaladin,
    KightWarrior,
    ArcherSummoner,
    PaladinPriest,
    WarriorMageRogue,
    WarriorMage,
    ArcherHunter,
    WarriorPaladin,
}

public enum ShootingType
{
    Straight,
    Parabola,
}

public enum EffectType
{
    None,
    Hit,
    UsingSkill,
}

public enum StatType
{
    None,
    Damage,
    HP,
    Defense,
}

public enum AttributeType
{
    None,
    AttackSpeedRate,
    CriticalProbability,
    CriticalMultiplication,
    Heal,
}

public enum SkillDamageType
{
    Impact,
    Tick,
}

public enum SkillTargetingType
{
    Normal,
    Range,
    Drop,
}

public enum SkillMoveType
{
    Throw,
    // 설치형
    Trigger,
}

public enum EquipmentType
{
    None,
    Arrow,
    Bow,
    Bracelet,
    Breast,
    Cape,
    Gauntlet,
    Helmet,
    Necklace,
    Ring,
    Shoes,
    Staff,
    Sword,
}

public enum CurrencyType
{
    None,
    Gold,
    Gem,
    ForgeTicket,
    ColleagueLevelUpStone,
    Exp,
    GoldDungeonTicket,
    ForgeDungeonTicket,
    GemDungeonTicket,
    ColleagueLevelUpDungeonTicket,
    AccelerationTicket,
    AbilityPoint,
    ColleagueSummonTicket,
}

public enum UIAnimationType
{
    None, Scale, VerticalPunch
}

public enum RewardType
{
    None,
    Gold,
    Gem,
    Exp,
    Equipment,
    ColleagueLevelUpStone,
    ForgeDungeonTicket,
    GemDungeonTicket,
    ForgeTicket,
    GoldDungeonTicket,
    ColleagueLevelUpDungeonTicket,
    AccelerationTicket,
    AbilityPoint,
    ColleagueSummonTicket,
    Colleague_Warrior_21,
}


// 반복퀘스트 증가값은 이 enum값으로 순서대로 설정함
public enum QuestType
{
    Event,
    ForgeDungeonLevelClear, // 반복 시 1씩 증가
    ForgeSummonCount,
    GemDungeonLevelClear, // 반복 시 1씩 증가
    MonsterKillCount,
    StageClear, // 반복 시 1씩 증가
    GoldDungeonLevelClear, // 반복 시 1씩 증가
    ColleagueLevelUpStoneDungeonLevelClear, // 반복 시 1씩 증가
}

public enum EventQuestType
{
    None,
    SkillUpgradeCount,
    ForgeLevelAchieve,
    EquipmentCompositeCount, // 장비 합성 횟수
    ForgeSummonSaleCount,
    AutoSkillButton,
    CastleUpgrade,
    ColleagueEquip,
    EquipmentEquip,
    Training,
    AutoForgeOpen,
    EquipColleagueAuto,
    PlayerLevel,
    ColleagueSummonCount_10,
    CastleDoorUpgradeButtonTouch,
    ColleagueLevelUp,
    ColleagueAdvancement,
    TouchEncyclopedia,
    TouchMission,
    ColleagueSummonCount_1,
    TouchBarracks,
}

public enum DungeonType
{
    None,
    ForgeTicketDungeon,
    GoldDungeon,
    GemDungeon,
    ColleagueLevelUpStoneDungeon,
    TrojanHorseDungeon,
}

public enum BattlePlaceType
{
    None,
    Stage,
    Dungeon,
}

public enum AdditionalStatType
{
    None,
    Equipment,
    Skill,
    Buff,
    Castle,
    Encyclopedia,
    Ability,
    Growth,
}

public enum ArithmeticStatType
{
    Base,
    Rate
}

public enum StatRangeType
{
    Individual,
    Communal,
}


public enum CastleQuestType
{
    None,
    CastleStageClear,
    CastleForgeSlotOpen,
    CastleRequiredCharacterLevel,
}

public enum SceneType
{
    TitleScene,
    MainScene,
}

public enum DamageType
{
    Normal,
    Critical,
    Castle,
}

public enum RedDotIDType
{
    ShowCastleButton,
    ShowSummonButton,
    ShowEquipmentButton,
    ShowSkillButton,
    ShowDungeonButton,
    CastleMission_1,
    CastleMission_2,
    CastleMission_3,
    CastleUpgradeButton,
    ColleagueSummonButton,
    ColleagueSummonButton_12,
    SkillSummonButton_15,
    SkillSummonButton_30,
    EquipmentRecommandButton,
    EquipmentCompositeButton,
    AutoSkillEquipButton,
    AutoSkillEnforeceButton,
    GotoForgeDungeonButton,
    GotoGemDungeonButton,
    GotoGoldDungeonButton,
    GotoColleagueLevelUpStoneDungeonButton,
    ForgeLevelUpButton,
    ForgePurchase,
    ForgeLevelUp,
    EncyclopediaButton,
    EncyclopediaEquipment,
    EncyclopediaSkill,
    ShowKingdomButton,
    DailyQuestButton,
    ShowColleagueButton,
    EncyclopediaColleague,
    AutoEquipColleagueButton,
    ColleagueAdvancement,
    ShowGrowthButton,
}

public enum MonsterType
{
    None,
    Normal,
    Boss,
    DungeonBoss,
}

public enum DailyQuestType
{
    Forge_Equipment,
    Summon_Colleague,
    Dungeon_GoldDungeon,
    Dungeon_ForgeDungeon,
    Dungeon_GemDungeon,
    Kill_Monster,
    LevelUP_Colleague,
    LevelUP_Skill,
    Dungeon_ColleagueLevelUpStoneDungeon,
}

public enum DialogueType
{
    None,
    FirstOpen,
    SummonForge,
    KillEnemies,
    ReSummonForge,
    SummonColleague,
    EquipColleague,
    LastOverview,
    LevelUpForge,
    LevelUpColleague,
    AdvanceUpColleague,
    ColleagueBook,
}

public enum AbilityOptionEffectType
{
    None,
    Damage,
    Defense,
    HP,
    AttackSpeedRate,
    CirticalProbability,
    CriticalMultiplication,
    Heal,
}

public enum FeatureType
{
    Level,
    Stage,
    Quest,
    BarracksLevel,
    Dialogue,
}

public enum FeatureID
{
    None,
    BottomBar_Castle,
    BottomBar_Summon,
    BottomBar_Colleague,
    BottomBar_Equipment,
    BottomBar_Dungeon,
    BottomBar_Kingdom,
    Summon_Colleague,
    Dungeon_Forge,
    Dungeon_Gold,
    Dungeon_Gem,
    Dungeon_ColleagueLevelUpStone,
    CastleDoorProbabilityButton,
    Forge_AutoForge,
    ColleagueSlot_1,
    ColleagueSlot_2,
    ColleagueSlot_3,
    ColleagueSlot_4,
    ColleagueSlot_5,
    ColleagueSlot_6,
    ColleagueSlot_7,
    ColleagueSlot_8,
    ColleagueSlot_9,
    ColleagueSlot_10,
    Kingdom_Barracks,
    Kingdom_Laboratory,
    Kingdom_KingsRoad,
    Kingdom_TheDoor,
    Kingdom_CastleBattle,
    Barracks_1,
    Barracks_2,
    Barracks_3,
    Barracks_4,
    Barracks_5,
    Encyclopedia,
    DailyQuest,
    BottomBar_Growth,
}

public enum GrowthButtonType
{
    None,
    Once = 1,
    FiveTimes = 5,
    TenTimes = 10,
}