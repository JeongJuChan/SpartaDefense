using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public const float HALF = 0.5f;
    public const float DEFAULT_PIXEL_PER_UNIT = 100;
    public const float PERCENT_MUTIPLY_VALUE = 0.01f;
    public const int PERCENT_DIVIDE_VALUE = 100;
    public const int PERCENT_TOTAL_VALUE = 10000;
    public const int PERCENT_UNIT_EQUIPMENT_VALUE = 1000;
    public const int PERCENT_TOTAL_EQUIPMENT_VALUE = 100000;
    public const int COMPOSIT_UNIT = 4;
    public const int MAX_HOUR = 24;
    public const int MAX_MINUTE = 60;
    public const int MAX_SECOND = 60;
    public const int MINUTE_PER_TICKET = 5;
    public const int PRICE_REPLACED_BY_GEM = 100;

    public static readonly Color32 DISABLE_COLOR = new Color32(142, 142, 142, 255);

    public const string BUTTON_LEVELUP = "레벨 업";
    public const string TITLE_LEVELUP = "장비 레벨 업";
    public const string CURRENCY_LACK_EQUIPMENTENFORCESTONE = "강화석이 부족합니다.";
    public const string EQUIP_MAX_LEVEL = "장비가 최대 레벨에 도달했습니다.";

    public const string WALL_TAG = "Wall";

    #region SaveStrings
    public const string ADS_SHOWN_TODAY_SAVE = "adsShownToday";
    public const string LAST_AD_SHOW_DATE_SAVE = "lastAdShowDate";
    public const string REWARD_TIME = "currentTime";
    public const string REWARD_DAY = "currentDay";

    public const string LastCheckTimeKey = "LastCheckTime";


    public const string SKILL_LEVEL = "_Level";
    public const string SKILL_CURRENT_EXP = "_CurrentExp";
    public const string SKILL_DAMAGE_PERCENT = "_DamagePerecent";
    public const string SKILL_ADDITIONAL_STAT_PERCENT = "_AdditionalStatPercent";
    public const string SKILL_MAX_EXP = "_MaxExp";
    public const string SKILL_EQUIP_INDEX = "_EquipIndex";
    public const string SKILL_IS_EQUIPPED = "_IsEquipped";

    public const string SLOT_NICKNAME = "_NickName";
    public const string SLOT_STAT_DATA = "_SlotStatData";
    public const string SLOT_ATTRIBUTE_STAT_DICT = "_AttributeStatDict";

    public const string DUNGEON_CHALLENGEABLE_LEVEL = "_DungeonChallengeableLevel";

    public const string EQUIPMENT_ACTIVE_NAME = "Activate_";
    public const string EQUIPMENT_DATA = "_Data";

    public const string USER_LEVEL = "UserLevel";
    public const string USER_EXP = "UserExp";

    public const string QUEST_MANAGER_DATA = "QuestManager_Data";
    public const string QUEST_PREFIX = "Quest_";
    public const string QUEST_COUNT = "_COUNT";
    public const string QUEST_CURRENT_INDEX = "_CurrentIndex";

    public const string CURRENT_MAIN_STAGE_NUM = "CurrentMainStageNum";
    public const string CURRENT_SUB_STAGE_NUM = "CurrentSubStageNum";
    public const string CURRENT_ROUTINE_STAGE_NUM = "CurrentRoutineStageNum";
    public const string CURRENT_DIFFICULTY = "CurrentDifficulty";
    public const string CURRENT_STAGE_INDEX = "CurrentStageIndex";
    public const string IS_BOSS_ENCOUNTERED = "IsBossEncountered";

    public const string FORGE_LEVEL = "ForgeLevel";
    public const string FORGE_CURRENT_EXP = "ForgeCurrentExp";

    public const string SUMMON_PREFIX = "Summon_";
    public const string CURRENT_SUMMON_EXP = "_CurrentSummonExp";
    public const string CURRENT_SUMMON_LEVEL = "_CurrentSummonLevel";
    public const string MAX_SUMMON_EXP = "_MaxSummonExp";
    public const string SUMMON_IS_LOCKED = "_IsLocked";

    public const string TOUCH_ENCYCLOPEDIA = "TouchEncyclopedia";
    public const string TOUCH_MISSION = "TouchMission";
    public const string TOUCH_BARRACKS = "TouchBarracks";
    public const string TRAINING_COUNT = "TrainingCount";

    public const string IS_LOCKED = "IsLocked";

    public const string IS_COLLEAGUE_LOCKED = "IsColleagueLocked";

    public const string IS_AUTO_FORGE_POSSIBLE = "AutoForgePossible";

    public const string CURRNET_ACTIVE_EXP_ELEMENT_INDEX = "CurrentActiveExpElementIndex";

    public const string IS_AUTO_SKILL_ACITVE = "AutoSkillActive";

    public const string COLLEAGUESLOTINDEX = "ColleagueSlotIndex";

    public const string IS_FIRST_TIME_INSTALLED = "IsFirstTimeInstalled";

    public const string COLLEAGUE_OPEN_SLOT_INDEX = "ColleagueOpenSlotIndex";

    public const string COMPLETED_QUEST = "_CompletedQuest";
    public const string CASTLE_CURRENT_LEVEL = "CurrentLevel";

    public const string IS_LEVEL_UP_PROCESSING_FORGE_PROBABILITY = "isLevelUpProcessing_ForgeProbability";
    public const string HAS_FORGE_PROBABILITY_TOUCHED = "hasForgeProbability_touched";
    public const string ACCELERATION_COMPLETE_TIME_FORGE_PROBABILITY = "accelerationCompleteTime_ForgeProbability";
    public const string ACCELERATION_START_DAY_FORGE_PROBABILITY = "accelerationStartDay_ForgeAbility";
    public const string HAS_COLLEAGUE_ADVANCED = "hasColleagueAdvanced";
    public const string HAS_FORGE_DOOR_TOUCHED = "hasForgeDoorTouched";

    public const string IS_TRANING_STATE = "isTraningState";
    public const string TRANING_COMPLETE_TIME = "trainingCompleteTime";
    public const string TRAINING_REWARD_COUNT = "trainingRewardCount";
    public const string TRAINING_START_TIME = "trainingStartTime";
    public const string TRAINING_START_DAY = "traningStartDay";
    public const string IS_TRAINING_DISABLED = "isTraninigDisabled";

    public const string IS_LEVEL_UP_PROCESSING_CASTLE_PROGRESSION = "isLevelUpProcessing_CastleProgression";
    public const string ACCELERATION_COMPLETE_TIME_CASTLE_PROGRESSION = "accelerationCompleteTime_CastleProgression";

    public const string IS_CASTLE_DOOR_LEVEL_UP_OPENED = "isCastleDoorLevelUpOpened";

    public const string COLLEAGUE_UPGRADABLE_DATA = "colleagueUpgradableData";
    public const string COLLEAGUE_UPGRADABLE_COUNT = "colleagueUpgradableCount";
    public const string COLLEAGUE_UPGRADABLE_DAMAGE = "colleagueUpgradableDamage";
    public const string COLLEAGUE_UPGRADABLE_HEALTH = "colleagueUpgradableHealth";
    public const string COLLEAGUE_UPGRADABLE_DEFENSE = "colleagueUpgradableDefense";
    public const string COLLEAGUE_UPGRADABLE_POWER = "colleagueUpgradablePower";

    public const string IS_COLLEAGUE_HAVE_BEEN_INSTANTIATED = "isColleagueHaveBeenInstantiated";

    public const string COLLEAGUE_EQUIP_INDEX = "colleagueEquipIndex";

    public const string IS_NEW_EQUIPMENT_SLOT_SOLD = "isNewEquipmentSlotSold";

    public const string IS_SHOWING_SUMMON_ANIM_STATE = "isShowingSummonAnimState";

    public const string DIALOGUE_TYPE_FIRST_OPEN = "DIALOGUE_TYPE_FIRST_OPEN_ACHIEVE";
    public const string DIALOGUE_TYPE_SUMMON_FORGE = "DIALOGUE_SUMMON_FORGE_ACHIEVE";
    public const string DIALOGUE_TYPE_KILL_ENEMIES = "DIALOGUE_KILL_ENEMIES_ACHIEVE";
    public const string DIALOGUE_TYPE_RE_SUMMON_FORGE = "DIALOGUE_RE_SUMMON_FORGE_ACHIEVE";
    public const string DIALOGUE_TYPE_SUMMON_COLLEAGUE = "DIALOGUE_SUMMON_COLLEAGUE_ACHIEVE";
    public const string DIALOGUE_TYPE_LEVEL_UP_FORGE = "DIALOGUE_LEVEL_UP_FORGE_ACHIEVE";
    public const string DIALOGUE_TYPE_LEVEL_UP_COLLEAGUE = "DIALOGUE_LEVEL_UP_COLLEAGUE_ACHIEVE";
    public const string DIALOGUE_TYPE_EQUIP_COLLEAGUE = "DIALOGUE_EQUIP_COLLEAGUE_ACHIEVE";
    public const string DIALOGUE_TYPE_LAST_OVERVIEW = "DIALOGUE_LAST_OVERVIEW_ACHIEVE";
    public const string DIALOGUE_TYPE_ADVANCE_UP_COLLEAGUE = "DIALOGUE_ADVANCE_UP_COLLEAGUE_ACHIEVE";
    public const string DIALOGUE_TYPE_COLLEAGUE_BOOK = "DIALOGUE_COLLEAGUE_BOOK_ACHIEVE";

    public const string DIALOGUE_TYPE_COMPLETE_MISSION = "DIALOGUE_COMPLETE_MISSION_ACHIEVE";
    public const string DIALOGUE_TYPE_ACQUIRE_EQUIPMENT = "DIALOGUE_ACQUIRE_EQUIPMENT_ACHIEVE";
    public const string DIALOGUE_TYPE_FORGE_TOUCH = "DIALOGUE_FORGE_TOUCH_ACHIEVE";
    public const string DIALOGUE_TYPE_SELL_ITEMS = "DIALOGUE_SELL_ITEMS_ACHIEVE";
    public const string IS_DIALOGUE_OPENED = "isDialogueOpened";
    public const string FirstOpen = "wasFirstOpenFinished";

    public const string GROWTH_LEVEL = "GrowthLevel";
    public const string GROWTH_LEVELS = "GrowthLevels";
    #endregion
}
