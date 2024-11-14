using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SOCSVLoader))]
public class SOCSVLoaderEditor : Editor
{
    private SOCSVLoader soCSVLoader;
    private const string defaultCSVPath = "CSV/";
    private const string defaultSOPath = "ScriptableObjects/";

    private void OnEnable()
    {
        soCSVLoader = (SOCSVLoader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Load All CSV to SO"))
        {
            LoadAll();
        }
    }

    private void LoadAll()
    {
        AttributeDataSOEditor.LoadCSVToSO(Resources.Load<AttributeDataSO>($"{defaultSOPath}Attributes/AttributeData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Attributes/Attributes CSV"));
        CastleDoorLevelDataSOEditor.LoadCSVToSO(Resources.Load<CastleDoorLevelDataSO>($"{defaultSOPath}Castle/CastleDoorLevelData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Castle/CastleDoorLevelData CSV"));
        CastleDoorRankProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<CastleDoorRankProbabilityDataSO>($"{defaultSOPath}Castle/CastleDoorRankProbabilityData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Castle/CastleProbability CSV"));
        CastleProgressionDataSOEditor.LoadCSVToSO(Resources.Load<CastleProgressionDataSO>($"{defaultSOPath}CastleProgressionDataSO/CastleProgressionData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Castle/CastleProgression CSV"));
        EffectDataSOEditor.LoadCSVToSO(Resources.Load<EffectDataSO>($"{defaultSOPath}Effects/EffectData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Effects/Effect CSV"));
        DungeonBossDataSOEditor.LoadCSVToSO(Resources.Load<DungeonBossDataSO>($"{defaultSOPath}Monsters/Dungeon/DungeonBossData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Dungeon/DungeonBoss CSV"));
        DungeonBossIncrementDataSOEditor.LoadCSVToSO(Resources.Load<DungeonBossIncrementDataSO>($"{defaultSOPath}Monsters/Dungeon/DungeonBossIncrementData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Dungeon/DungeonBoss CSV"));
        MonsterCoreDataSOEditor.LoadCSVToSO(Resources.Load<MonsterCoreInfoDataSO>($"{defaultSOPath}Monsters/MonsterCoreInfoData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Monsters CSV"));
        BossIncrementDataSOEditor.LoadCSVToSO(Resources.Load<BossIncrementDataSO>($"{defaultSOPath}Stage/BossIncrementData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        MonsterIncrementDataSOEditor.LoadCSVToSO(Resources.Load<MonsterIncrementDataSO>($"{defaultSOPath}Stage/MonsterIncrementData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        MonsterBaseDataSOEditor.LoadCSVToSO(Resources.Load<MonsterBaseStatDataSO>($"{defaultSOPath}Monsters/Stage/MonsterBaseStatData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        BossBaseStatDataEditor.LoadCSVToSO(Resources.Load<BossBaseStatDataSO>($"{defaultSOPath}Monsters/Stage/BossBaseStatData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        MonsterRewardDataSOEditor.LoadCSVToSO(Resources.Load<MonsterRewardDataSO>($"{defaultSOPath}Reward/MonsterRewardData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        MonsterRewardIncrementDataSOEditor.LoadCSVToSO(Resources.Load<MonsterRewardIncrementDataSO>($"{defaultSOPath}Reward/MonsterRewardIncrementData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        BossRewardDataSOEditor.LoadCSVToSO(Resources.Load<BossRewardDataSO>($"{defaultSOPath}Reward/BossRewardData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        BossRewardIncrementDataSOEditor.LoadCSVToSO(Resources.Load<BossRewardIncrementDataSO>($"{defaultSOPath}Reward/BossRewardIncrementData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        SkillDataSOEditor.LoadCSVToSO(Resources.Load<SkillDataSO>($"{defaultSOPath}Skills/SkillData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Skills/Skills CSV"));
        SkillIncrementDataSOEditor.LoadCSVToSO(Resources.Load<SkillIncrementDataSO>($"{defaultSOPath}Skills/SkillIncrementData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Skills/SkillIncrement CSV"));
        SlotDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueDataSO>($"{defaultSOPath}Colleague/ColleagueData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ColleagueData CSV"));
        SlotStatDataSOEditor.LoadCSVToSO(Resources.Load<SlotStatDataSO>($"{defaultSOPath}Colleague/ForgeEquipmentData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ForgeEquipment CSV"));
        MonsterCountDataSOEditor.LoadCSVToSO(Resources.Load<MonsterCountDataSO>($"{defaultSOPath}Stage/MonsterCountData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Stages/StageMonstersCount CSV"));
        UserLevelDataSOEditor.LoadCSVToSO(Resources.Load<UserLevelDataSO>($"{defaultSOPath}UserLevel/UserLevelData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}UserLevel/UserLevel CSV"));
        UnlockDataSOEditor.LoadCSVToSO(Resources.Load<UnlockDataSO>($"{defaultSOPath}UnlockDataSO/UnlockData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Unlock/UnlockData"));
        SummonProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<SummonProbabilityDataSO>($"{defaultSOPath}SummonProbabilityDataSO/EquipmentSummonProbabilityDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Summon/EquipmentProbability CSV"));
        SummonProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<SummonProbabilityDataSO>($"{defaultSOPath}SummonProbabilityDataSO/SkillSummonProbabilityDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Summon/SkillProbability CSV"));
        SummonProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<SummonProbabilityDataSO>($"{defaultSOPath}SummonProbabilityDataSO/ColleagueSummonProbabilityDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Summon/ColleagueProbability CSV"));
        /*EquipmentBaseDataSOEditor.LoadCSVToSO(Resources.Load<EquipmentBaseDataSO>($"{defaultSOPath}EquipmentBaseDataSO/ArmorBaseData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Equipment/EquipmentStats CSV"));
        EquipmentBaseDataSOEditor.LoadCSVToSO(Resources.Load<EquipmentBaseDataSO>($"{defaultSOPath}EquipmentBaseDataSO/BowBaseData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Equipment/EquipmentStats CSV"));
        EquipmentBaseDataSOEditor.LoadCSVToSO(Resources.Load<EquipmentBaseDataSO>($"{defaultSOPath}EquipmentBaseDataSO/GloveBaseData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Equipment/EquipmentStats CSV"));
        EquipmentBaseDataSOEditor.LoadCSVToSO(Resources.Load<EquipmentBaseDataSO>($"{defaultSOPath}EquipmentBaseDataSO/ShoesBaseData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Equipment/EquipmentStats CSV"));*/
        BarracksLevelDataSOEditor.LoadCSVToSO(Resources.Load<BarracksLevelDataSO>($"{defaultSOPath}Kingdom/Barracks/BarracksLevelData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Kingdom/Barracks/BarracksLevelData CSV"));
        TrainingSoldierDataSOEditor.LoadCSVToSO(Resources.Load<TrainingSoldierDataSO>($"{defaultSOPath}Kingdom/Barracks/TrainingSoldierData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Kingdom/Barracks/TrainingSoldierData CSV"));
        DialogueDataSOEditor.LoadCSVToSO(Resources.Load<DialogueDataSO>($"{defaultSOPath}DialogueDataSO/DialogueData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Dialogue/DialogueData CSV"));
        CastleDurationDataSOEditor.LoadCSVToSO(Resources.Load<CastleDoorDurationDataSO>($"{defaultSOPath}Castle/CastleDoorDurationData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Castle/CastleDoorLevelUpDuration CSV"));
        ColleagueSummonExpDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueSummonExpDataSO>($"{defaultSOPath}Colleague/ColleagueSummonExpData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ColleagueSummonExpData CSV"));
        ColleaguePartDataSOEditor.LoadCSVToSO(Resources.Load<ColleaguePartDataSO>($"{defaultSOPath}Colleague/ColleaguePartData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ColleaguePartData CSV"));
        ColleagueStatDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueStatDataSO>($"{defaultSOPath}Colleague/ColleagueStatData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ColleagueStatData CSV"));
        ColleagueLevelUpDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueLevelUpDataSO>($"{defaultSOPath}Colleague/ColleagueLevelUpData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ColleagueLevelUpData CSV"));
        ColleagueProjectileDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueProjectileDataSO>($"{defaultSOPath}Colleague/ColleagueProjectileData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Colleague/ColleagueProjectileData CSV"));
        ForgeEquipmentDataSOEditor.LoadCSVToSO(Resources.Load<ForgeEquipmentDataSO>($"{defaultSOPath}Equipments/ForgeEquipmentData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Equipments/ForgeEquipmentData CSV"));
        RepeatQuestUpgradeValueDataSOEditor.LoadCSVToSO(Resources.Load<RepeatQuestUpgradeValueDataSO>($"{defaultSOPath}RepeatQuestUpgradeValueDataSO/RepeatQuestUpgradeValueData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Quest/RepeatQuestData CSV"));
        /*ColleagueEncyclopediaDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueEncyclopediaDataSO>($"{defaultSOPath}Encyclopedia/ColleagueEncyclopediaData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Encyclopedia/ColleagueEncyclopediaData CSV"));
        ColleagueEncyclopediaIncrementDataSOEditor.LoadCSVToSO(Resources.Load<ColleagueEncyclopediaIncrementDataSO>($"{defaultSOPath}Encyclopedia/ColleagueEncyclopediaIncrementData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Encyclopedia/ColleagueEncyclopediaData CSV"));*/
        GrowthDataSOEditor.LoadCSVToSO(Resources.Load<GrowthDataSO>($"{defaultSOPath}Growth/GrowthDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Growth/GrowthData CSV"));
        EnumToKRSOEditor.LoadCSVToSO(Resources.Load<EnumToKRSO>($"{defaultSOPath}ToKR/EnumToKR"),
            Resources.Load<TextAsset>($"{defaultCSVPath}ToKR/EnumToKR CSV"));
    }
}
