using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CastleClan : MonoBehaviour
{
    [SerializeField] private SlotStatDataSO slotStatDataSO;

    private Dictionary<ColleagueType, Hero> colleagueDictByColleagueType = new Dictionary<ColleagueType, Hero>();
    private Dictionary<int, Hero> colleagueDictByIndex = new Dictionary<int, Hero>();

    private MonsterSpawnData monsterSpawnData;
    private Func<List<Monster>> OnGetTargetFunc;

    [SerializeField] private Transform[] heroPivots;

    [SerializeField] private Transform frontHeroGroupTransform;
    [SerializeField] private Transform allHeroGroupTransform;

    [SerializeField] private Vector2 underLevel3Pos;
    [SerializeField] private Vector2 level4Pos;

    [SerializeField] private Transform rtanParent;

    private EffectSpawner effectSpawner;

    private int preHeroCount = 0;

    private SlotEquipmentForger slotEquipmentForger;
    private ColleagueSpawner colleagueSpawner;

    private Vector2 standardScreenSize = new Vector2(2960, 1440);

    private ColleagueManager colleagueManager;

    private Dictionary<int, List<KeyValuePair<StatType, BigInteger>>> colleagueStatDict = 
        new Dictionary<int, List<KeyValuePair<StatType, BigInteger>>>();

    public event Action<ColleagueInfo> OnUpdateColleague;

    private SkillManager skillManager;

    public void Init()
    {
        InitHeroModifiedPos();

        skillManager = FindAnyObjectByType<SkillManager>();
        skillManager.OnPlayUsingSkillParticle += PlayUsingSkillParticle;
        effectSpawner = FindAnyObjectByType<EffectSpawner>();
        slotEquipmentForger = FindAnyObjectByType<SlotEquipmentForger>(FindObjectsInactive.Include);
        colleagueSpawner = FindAnyObjectByType<ColleagueSpawner>();
        //slotEquipmentForger.OnHeroInstantiated += AddHero;
        colleagueSpawner.OnHeroInstantiated += AddHero;
        UI_Colleague ui_Colleague = UIManager.instance.GetUIElement<UI_Colleague>();

        ui_Colleague.OnEquipColleague += SetColleague;
        ui_Colleague.OnUnEquipColleague += RemoveColleague;

        StatViewerHelper.instance.OnCastleLevelChanged += ChangeHeroesPos;

        colleagueManager = FindAnyObjectByType<ColleagueManager>();
    }

    #region HeroListMethods
    public Dictionary<ColleagueType, Hero> GetHeroes()
    {
        return colleagueDictByColleagueType;
    }

    public bool GetIsHeroExist(ColleagueType colleagueType)
    {
        return colleagueDictByColleagueType.ContainsKey(colleagueType);
    }

    private bool GetIsColleagueExist(int index)
    {
        return colleagueDictByIndex.ContainsKey(index);
    }

    public bool TryGetShooter<T>(ColleagueType slotType, out T shooter) where T : Hero
    {
        if (colleagueDictByColleagueType.ContainsKey(slotType))
        {
            shooter = colleagueDictByColleagueType[slotType] as T;
        }
        else
        {
            shooter = null;
        }

        return shooter;
    }

    private int GetHeroesCount()
    {
        return colleagueDictByColleagueType.Count;
    }

    private void PlayUsingSkillParticle(ColleagueType colleagueType)
    {
        if (colleagueDictByColleagueType.ContainsKey(colleagueType))
        {
            colleagueDictByColleagueType[colleagueType].PlayUsingSkillParticle();
        }
    }

    // private bool GetHeroExist(SlotType slotType)
    // {
    //     return heroesDict.ContainsKey(slotType);
    // }

    private void SetColleague(int slotIndex, int index, bool isLastColleague)
    {
        if (!GetIsColleagueExist(index))
        {
            colleagueSpawner.InstantiateHero(index);
        }

        if (!colleagueStatDict.ContainsKey(index))
        {
            colleagueStatDict.Add(index, new List<KeyValuePair<StatType, BigInteger>> {
                new KeyValuePair<StatType, BigInteger>(StatType.Damage, new BigInteger(0)),
                new KeyValuePair<StatType, BigInteger>(StatType.HP, new BigInteger(0)),
                new KeyValuePair<StatType, BigInteger>(StatType.Defense, new BigInteger(0))
            });
        }

        OnUpdateColleague?.Invoke(colleagueDictByIndex[index].GetColleagueInfo());

        ColleagueType colleagueType = colleagueDictByIndex[index].GetColleagueType();

        UpdateColleagueStatDict(index);

        StatDataHandler.Instance.AddAttributeEvent(AttributeType.AttackSpeedRate, colleagueDictByIndex[index].UpdateAttackSpeedRate);
        colleagueDictByIndex[index].UpdateAttackSpeedRate(StatDataHandler.Instance.GetAttributeValue(AttributeType.AttackSpeedRate));

        StatDataHandler.Instance.ModifyStats(ArithmeticStatType.Base, colleagueType, colleagueStatDict[index], isLastColleague, 
            isLastColleague);
        SetHeroPosition(colleagueDictByIndex[index], slotIndex);

        colleagueDictByIndex[index].gameObject.SetActive(true);
        colleagueDictByIndex[index].UpdateMyPos();
        colleagueDictByIndex[index].TryShoot();
    }

    public void UpdateColleagueStatDict(int index)
    {
        ColleagueUpgradableData colleagueUpgradableData = colleagueManager.GetColleagueUpgradableData(index);

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            switch (statType)
            {
                case StatType.HP:
                    colleagueStatDict[index][(int)statType - 1] =
                        new KeyValuePair<StatType, BigInteger>(statType, colleagueUpgradableData.health);
                    break;
                case StatType.Damage:
                    colleagueStatDict[index][(int)statType - 1] =
                        new KeyValuePair<StatType, BigInteger>(statType, colleagueUpgradableData.damage);
                    break;
                case StatType.Defense:
                    colleagueStatDict[index][(int)statType - 1] =
                        new KeyValuePair<StatType, BigInteger>(statType, colleagueUpgradableData.defense);
                    break;
            }
        }
    }

    private void AddHero(Hero hero)
    {
        ColleagueInfo slotInfo = hero.GetSlotInfo();

        SlotHeroData slotHeroData = ResourceManager.instance.slotHeroData.GetResource(slotInfo);
        hero.UpdateSlotHeroData(slotHeroData);

        if (!colleagueDictByColleagueType.ContainsKey(slotInfo.colleagueType))
        {
            colleagueDictByColleagueType.Add(slotInfo.colleagueType, hero);
        }

        int index = hero.GetIndex();
        if (!colleagueDictByIndex.ContainsKey(index))
        {
            colleagueDictByIndex.Add(index, hero);
        }

        int heroCount = GetHeroesCount();
        if (preHeroCount != heroCount)
        {
            effectSpawner.UpdatePoolCount(heroCount);
        }

        preHeroCount = heroCount;

        hero.SetDirectionData(monsterSpawnData);
        hero.SetGetTargetFunc(OnGetTargetFunc);
    }

    private void SetHeroPosition(Hero hero, int slotIndex)
    {
        if (hero.GetColleagueType() == ColleagueType.Rtan_Rare)
        {
            hero.transform.SetParent(rtanParent);
        }
        else
        {
            hero.transform.SetParent(heroPivots[slotIndex - 1]);
        }

        hero.transform.localPosition = Vector2.zero;
    }

    private void RemoveColleague(int index)
    {
        ColleagueType colleagueType = colleagueDictByIndex[index].GetColleagueType();

        UpdateColleagueStatDict(index);

        StatDataHandler.Instance.RemoveAttributeEvent(AttributeType.AttackSpeedRate, colleagueDictByIndex[index].UpdateAttackSpeedRate);
        StatDataHandler.Instance.ModifyStats(ArithmeticStatType.Base, colleagueType, colleagueStatDict[index], false, true);


        colleagueDictByIndex[index].gameObject.SetActive(false);
        // TODO: 풀링 할당 해제 해야함
        //ColleagueType slotType = hero.GetSlotInfo().colleagueType;
        //heroesDict.Remove(slotType);
    }
    #endregion

    #region SettingsForHeroMethods
    public void SetHeroesDirectionData(MonsterSpawnData monsterSpawnData)
    {
        this.monsterSpawnData = monsterSpawnData;
    }

    public void SetHeroesGetTargetFunc(Func<List<Monster>> getTargetFunc)
    {
        this.OnGetTargetFunc = getTargetFunc;
    }


    private void InitHeroModifiedPos()
    {
        float width = Screen.width / standardScreenSize.x;
        float height = Screen.height / standardScreenSize.y;
        /*
                underLevel3Pos.x *= width;
                underLevel3Pos.y *= height;

                level4Pos.x *= width;
                level4Pos.y *= height;*/
    }

    public void ChangeHeroesPos(int level) //TODO: Castle 진급하면 Castle Level 줘야함
    {
        if (level < 3)
        {
            allHeroGroupTransform.localPosition = underLevel3Pos;
        }
        else
        {
            allHeroGroupTransform.localPosition = level4Pos;
        }
    }
    #endregion
}
