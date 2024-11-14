using Keiwando.BigInteger;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum EncyclopediaType
{
    Equipment,
    Skill,
    Colleague,
}

// EncyclopediaCategoryData 클래스
// Category의 데이터를 저장하는 클래스
// Category는 이름, 레벨, 레벨당 증가 능력치, SlotDatas를 가지고 있다.
// SlotDatas는 EncyclopediaSlotData를 List로 가지고 있다.
// SlotDatas는 Slot의 데이터를 저장하는 List이다.
// Slot의 데이터는 아이템의 이름을 가진다.
public class EncyclopediaCategoryData
{
    public string name;
    public int level;
    public int levelUpAbility;
    public StatType statType;
    public ArithmeticStatType arithmeticStatType;
    public List<EncyclopediaSlotData> slotDatas;

    public EncyclopediaCategoryData(string name, StatType statType, int levelUpAbility, ArithmeticStatType arithmeticStatType, List<EncyclopediaSlotData> slotDatas)
    {
        this.name = name;
        this.statType = statType;
        this.levelUpAbility = levelUpAbility;
        this.arithmeticStatType = arithmeticStatType;
        this.slotDatas = slotDatas;

        level = ES3.Load<int>("EncyclopediaCategoryData_" + name + "_Level", 0);
    }

    public void UpdaTeLevel()
    {
        level = ES3.Load<int>("EncyclopediaCategoryData_" + name + "_Level", 0);
    }
}

// EncyclopediaSlotData 클래스
// Slot의 데이터를 저장하는 클래스
// Slot은 아이템의 이름을 가지고 있다.
public class EncyclopediaSlotData
{
    public string itemName;

    public EncyclopediaSlotData(string itemName)
    {
        this.itemName = itemName;
    }
}

public class EncyclopediaUIHandler : MonoBehaviour
{
    private EncyclopediaDataHandler encyclopediaDataHandler;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject encyclopediaPanel;
    [SerializeField] private EncyclopediaCategory encyclopediaCategoryPrefab;
    [SerializeField] private Transform encyclopediaSlotArea;
    [SerializeField] private Button equipmentSwitchingButton;
    [SerializeField] private Button skillSwitchingButton;
    [SerializeField] private ColleagueDataSO colleagueDataSO;

    private Dictionary<EncyclopediaType, List<EncyclopediaCategory>> encyclopediaCategories = new Dictionary<EncyclopediaType, List<EncyclopediaCategory>>();
    private Dictionary<EncyclopediaType, List<EncyclopediaCategoryData>> encyclopediaCategoryDatas = new Dictionary<EncyclopediaType, List<EncyclopediaCategoryData>>();
    private EncyclopediaType encyclopediaType = EncyclopediaType.Equipment;

    private Dictionary<EncyclopediaType, Dictionary<EncyclopediaCategory, (bool HasLevelUp, Rank Rank)>> categoryCache = new Dictionary<EncyclopediaType, Dictionary<EncyclopediaCategory, (bool HasLevelUp, Rank Rank)>>();

    private Dictionary<ColleagueType, HashSet<EncyclopediaSlot>> colleagueEncyclopediaSlotsDict = new Dictionary<ColleagueType, HashSet<EncyclopediaSlot>>();

    // 레벨업이 가능한 Category를 저장하여 RedDot을 표시할 때 사용한다.
    // Category 중 레벨업이 가능한 Category들과 Category의 EncyclopediaType을 저장한다.
    // Category의 레벨업이 가능한지 확인하는 함수를 만들어서 레벨업이 가능한 Category들을 저장하는 Dictionary를 만든다.
    Dictionary<EncyclopediaType, List<string>> levelUpAvailableCategories = new Dictionary<EncyclopediaType, List<string>>();

    // 게임 시작 시 Category를 생성하는 함수
    // 게임 시작 시 Start에서 Category를 생성하고, 각 Category에 해당하는 Slot을 생성한다.
    // Category를 생성한 뒤 재활용을 위해 List에 추가한다.
    // Category의 데이터 세팅은 SetData 함수를 통해 이루어진다.
    // Encyclopedia는 장비, 스킬 총 2가지의 Type을 가지고 있다.
    // 세팅이 할 때 EncyclopediaDataHandler에서 SetData에서 매개변수로 받은 Type에 맞는 데이터를 가져와서 세팅한다.
    // 세팅은 처음 OnEnable될 때와 OnEnable 될 때는 Equipment로 세팅한다.
    // EquipmentSwitching Button과 SkillSwitching Button을 누르면 각각의 Type으로 세팅한다.



    public void Init()
    {
        encyclopediaDataHandler = EncyclopediaDataHandler.Instance;
        encyclopediaDataHandler.OnSlotLevelChangeEvent += CheckSlotCountChange;
        UIManager.instance.GetUIElement<UI_Colleague>().OnUpdateColleagueLevel += OnUpdateEncyclopediaSlotLevelText;
        closeButton.onClick.AddListener(() => ActivateSelf(false));
        SetData(EncyclopediaType.Colleague);
        ActivateSelf(false);

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.TouchEncyclopedia,
            () => QuestManager.instance.UpdateCount(EventQuestType.TouchEncyclopedia,
            PlayerPrefs.HasKey(Consts.TOUCH_ENCYCLOPEDIA) ? 1 : 0, -1));
    }

    private void ActivateSelf(bool isActive)
    {
        closeButton.gameObject.SetActive(isActive);
        encyclopediaPanel.SetActive(isActive);
    }

    private void Start()
    {
        /*equipmentSwitchingButton.onClick.AddListener(() => CategorySwitching(EncyclopediaType.Equipment));
        skillSwitchingButton.onClick.AddListener(() => CategorySwitching(EncyclopediaType.Skill));
        closeButton.onClick.AddListener(() => encyclopediaPanel.SetActive(false));
        CreateCategory();
        SetData(EncyclopediaType.Equipment);
        SetData(EncyclopediaType.Skill);

        levelUpAvailableCategories[EncyclopediaType.Equipment] = ES3.Load<List<string>>($"LevelUpAvailableCategories_{EncyclopediaType.Equipment}", new List<string>());
        levelUpAvailableCategories[EncyclopediaType.Skill] = ES3.Load<List<string>>($"LevelUpAvailableCategories_{EncyclopediaType.Skill}", new List<string>());
        */
        levelUpAvailableCategories[EncyclopediaType.Colleague] = ES3.Load<List<string>>($"LevelUpAvailableCategories_{EncyclopediaType.Colleague}", new List<string>());
    }

    public void OnClick_EncyclopediaOpen()
    {
        PlayerPrefs.SetInt(Consts.TOUCH_ENCYCLOPEDIA, 1);
        QuestManager.instance.UpdateCount(EventQuestType.TouchEncyclopedia, PlayerPrefs.HasKey(Consts.TOUCH_ENCYCLOPEDIA) ? 1 : 0, -1);

        //SetData(EncyclopediaType.Equipment);
        //SetData(EncyclopediaType.Colleague);
        // y값을 0으로 초기화
        encyclopediaSlotArea.localPosition = new Vector3(encyclopediaSlotArea.localPosition.x, 0, encyclopediaSlotArea.localPosition.z);

        ActivateSelf(true);
    }

    private void CreateCategory()
    {
        encyclopediaCategories.Add(EncyclopediaType.Equipment, new List<EncyclopediaCategory>());
        encyclopediaCategories.Add(EncyclopediaType.Skill, new List<EncyclopediaCategory>());

        for (int i = 0; i < 10; i++)
        {
            EncyclopediaCategory category_Equipment = Instantiate(encyclopediaCategoryPrefab, encyclopediaSlotArea);
            EncyclopediaCategory category_Skill = Instantiate(encyclopediaCategoryPrefab, encyclopediaSlotArea);
            category_Equipment.LevelUpEvent += UpdateUIActual;
            category_Equipment.LevelUpAbilityEvent += AddLevelUpAvailableCategory;
            category_Skill.LevelUpEvent += UpdateUIActual;
            category_Skill.LevelUpAbilityEvent += AddLevelUpAvailableCategory;
            encyclopediaCategories[EncyclopediaType.Equipment].Add(category_Equipment);
            encyclopediaCategories[EncyclopediaType.Skill].Add(category_Skill);
        }
    }

    private void OnUpdateEncyclopediaSlotLevelText(ColleagueType colleagueType, int level)
    {
        if (!colleagueEncyclopediaSlotsDict.ContainsKey(colleagueType))
        {
            return;
        }

        //bool isLevelUpPossible = false;

        foreach (EncyclopediaSlot encyclopediaSlot in colleagueEncyclopediaSlotsDict[colleagueType])
        {
            encyclopediaSlot.UpdateLevel(level);

            /*if (encyclopediaSlot.GetIsLevelUpAvailable())
            {
                isLevelUpPossible = true;
            }*/
            // TODO: 보기

            CheckSlotCountChange(EncyclopediaType.Colleague, encyclopediaSlot.GetColleagueEncyclopediaType().ToString());
        }

        CheckLevelUpAvailableCategories(EncyclopediaType.Colleague);
    }

    public void SetData(EncyclopediaType type)
    {
        encyclopediaType = type;

        if (!encyclopediaCategories.ContainsKey(type))
        {
            encyclopediaCategories.Add(type, new List<EncyclopediaCategory>());
            if (type == EncyclopediaType.Colleague)
            {
                foreach (ColleagueEncyclopediaData data in encyclopediaDataHandler.GetColleagueEncyclopediaDatas().Values)
                {
                    EncyclopediaCategory category = Instantiate(encyclopediaCategoryPrefab, encyclopediaSlotArea);
                    int level = ES3.Load<int>($"EncyclopediaCategoryData_{data.colleagueEncyclopediaType}_Level", 0, ES3.settings);

                    category.LevelUpEvent += UpdateUIActual;
                    category.LevelUpAbilityEvent += AddLevelUpAvailableCategory;
                    category.OnGetColleagueEncyclopediaIncrementData += GetColleagueEncyclopediaIncrementData;

                    ColleagueEncyclopediaIncrementData preColleagueEncyclopediaIncrementData = level == 0 ? default :
                        encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(data.colleagueEncyclopediaType, level - 2);

                    ColleagueEncyclopediaIncrementData colleagueEncyclopediaIncrementData =
                        encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(data.colleagueEncyclopediaType, level - 1);

                    ColleagueEncyclopediaIncrementData nextColleagueEncyclopediaIncrementData =
                        encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(data.colleagueEncyclopediaType, level);

                    category.UpdateLevelUpIncrementStats(preColleagueEncyclopediaIncrementData, colleagueEncyclopediaIncrementData,
                        nextColleagueEncyclopediaIncrementData);
                    category.InitColleagueEncyclopediaData(data, level, nextColleagueEncyclopediaIncrementData.goalLevelEachElement,
                        colleagueDataSO.GetColleagueInfoByColleagueType, TryAddElement);
                    category.InitInteraction(nextColleagueEncyclopediaIncrementData.increment == 0);

                    category.Init();
                    encyclopediaCategories[type].Add(category);

                    BigInteger totalStat = 0;

                    for (int i = 0; i < level; i++)
                    {
                        ColleagueEncyclopediaIncrementData tempColleagueEncyclopediaIncrementData =
                        encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(data.colleagueEncyclopediaType, i);

                        totalStat += tempColleagueEncyclopediaIncrementData.increment;
                    }

                    StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Encyclopedia, data.statType,
                        totalStat, true);
                }
            }
        }
        /*else
        {
            if (type == EncyclopediaType.Colleague)
            {
                foreach (ColleagueEncyclopediaData data in encyclopediaDataHandler.GetColleagueEncyclopediaDatas().Values)
                {
                    foreach (EncyclopediaCategory category in encyclopediaCategories[type])
                    {
                        int level = ES3.Load<int>($"EncyclopediaCategoryData_{data.colleagueEncyclopediaType}_Level", 0, ES3.settings);

                        ColleagueEncyclopediaIncrementData colleagueEncyclopediaIncrementData =
                            encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(data.colleagueEncyclopediaType, level);

                        ColleagueEncyclopediaIncrementData nextColleagueEncyclopediaIncrementData =
                            encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(data.colleagueEncyclopediaType, level + 1);

                        category.SetData(data, level);

                        category.UpdateLevelUpIncrementStats(colleagueEncyclopediaIncrementData,
                            nextColleagueEncyclopediaIncrementData.level == 0);
                    }
                }
            }

            return;

            encyclopediaCategoryDatas[encyclopediaType] = encyclopediaDataHandler.GetCategoryDatas(encyclopediaType);

            if (encyclopediaCategories[type].Count < encyclopediaCategoryDatas[type].Count)
            {
                for (int i = encyclopediaCategories[type].Count; i < encyclopediaCategoryDatas[type].Count; i++)
                {
                    EncyclopediaCategory category = Instantiate(encyclopediaCategoryPrefab, encyclopediaSlotArea);
                    category.LevelUpEvent += UpdateUIActual;
                    category.LevelUpAbilityEvent += AddLevelUpAvailableCategory;
                    encyclopediaCategories[type].Add(category);
                }
            }

            // 현재 encyclopediaType에 해당하지 않는 Category는 비활성화한다.
            for (int i = 0; i < encyclopediaCategories.Count; i++)
            {
                if (i != (int)type)
                {
                    for (int j = 0; j < encyclopediaCategories[(EncyclopediaType)i].Count; j++)
                    {
                        encyclopediaCategories[(EncyclopediaType)i][j].gameObject.SetActive(false);
                    }
                }
            }

            for (int i = 0; i < encyclopediaCategories[type].Count; i++)
            {
                if (i < encyclopediaCategoryDatas[type].Count)
                {
                    encyclopediaCategories[type][i].gameObject.SetActive(true);
                    encyclopediaCategoryDatas[type][i].UpdaTeLevel();
                    encyclopediaCategories[type][i].SetData(encyclopediaCategoryDatas[type][i], type);

                    if (!encyclopediaCategories[type][i].initialized)
                        encyclopediaCategories[type][i].Init();
                }
                else
                {
                    encyclopediaCategories[type][i].gameObject.SetActive(false);
                }
            }

            UpdateUIActual();

            CheckLevelUpAvailableCategories(type);
        }*/
    }

    private void TryAddElement(ColleagueType colleagueType, EncyclopediaSlot encyclopediaSlot)
    {
        if (!colleagueEncyclopediaSlotsDict.ContainsKey(colleagueType))
        {
            colleagueEncyclopediaSlotsDict.Add(colleagueType, new HashSet<EncyclopediaSlot>());
        }

        //encyclopediaSlot.LevelUp
        //colleagueEncyclopediaSlotsDict[colleagueType]
        colleagueEncyclopediaSlotsDict[colleagueType].Add(encyclopediaSlot);
    }

    private ColleagueEncyclopediaIncrementData GetColleagueEncyclopediaIncrementData(
        ColleagueEncyclopediaType colleagueEncyclopediaType, int level)
    {
        ColleagueEncyclopediaIncrementData colleagueEncyclopediaIncrementData =
                        encyclopediaDataHandler.GetColleagueEncyclopediaIncrementData(colleagueEncyclopediaType, level);
        return colleagueEncyclopediaIncrementData;
    }

    private void CheckSlotCountChange(EncyclopediaType type, string categoryName)
    {
        /*encyclopediaCategories[type].Find(category => category.categoryName == categoryName)
        .CheckLevelUpAvailable(encyclopediaCategoryDatas[type].Find(data => data.name == categoryName));*/

        encyclopediaCategories[type].Find(category => category.categoryName == categoryName)
        .CheckLevelUpAvailable();

        CheckLevelUpAvailableCategories(type);
    }

    private void CheckLevelUpAvailableCategories(EncyclopediaType type)
    {
        if (!levelUpAvailableCategories.ContainsKey(type))
        {
            levelUpAvailableCategories[type] = new List<string>();
        }

        if (levelUpAvailableCategories[type].Count > 0)
        {
            NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaButton, true);
            switch (type)
            {
                case EncyclopediaType.Equipment:
                    NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaEquipment, true);
                    break;
                case EncyclopediaType.Skill:
                    NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaSkill, true);
                    break;
                case EncyclopediaType.Colleague:
                    NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaColleague, true);
                    break;
            }
        }
        else
        {
            NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaButton, false);
            switch (type)
            {
                case EncyclopediaType.Equipment:
                    NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaEquipment, false);
                    break;
                case EncyclopediaType.Skill:
                    NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaSkill, false);
                    break;
                case EncyclopediaType.Colleague:
                    NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaColleague, false);
                    break;
            }
        }
    }

    private void AddLevelUpAvailableCategory(EncyclopediaType type, string categoryName, bool isLevelUpAvailable)
    {
        if (!levelUpAvailableCategories.ContainsKey(type))
        {
            levelUpAvailableCategories[type] = new List<string>();
        }

        if (isLevelUpAvailable)
        {
            if (!levelUpAvailableCategories[type].Contains(categoryName))
            {
                levelUpAvailableCategories[type].Add(categoryName);
            }
        }
        else
        {
            if (levelUpAvailableCategories[type].Contains(categoryName))
            {
                levelUpAvailableCategories[type].Remove(categoryName);
            }
        }

        ES3.Save<List<string>>($"LevelUpAvailableCategories_{type}", levelUpAvailableCategories[type], ES3.settings);
    }

    public void CheckLevelUp(string categoryName)
    {
        encyclopediaCategories[encyclopediaType].Find(category => category.categoryName == categoryName).CheckLevelUpAvailable();
    }

    public void CategorySwitching(EncyclopediaType type)
    {
        encyclopediaSlotArea.localPosition = new Vector3(encyclopediaSlotArea.localPosition.x, 0, encyclopediaSlotArea.localPosition.z);

        SetData(type);
    }

    private void UpdateUIActual()
    {
        CacheCategoryInfo(); // Make sure we have the latest data

        var categoryCache = this.categoryCache[encyclopediaType];
        var currentCategories = encyclopediaCategories[encyclopediaType];

        var sortedCategories = categoryCache
            .OrderByDescending(pair => pair.Value.HasLevelUp) // Prioritize categories with level-ups
            .ThenBy(pair => pair.Value.Rank) // Maintain the original sorting order within each group
            .Select(pair => pair.Key)
            .ToList();

        var withLevelUp = currentCategories.Where(cat => cat.isLevelUpAvailable).ToArray();
        var withoutLevelUp = currentCategories.Except(withLevelUp).ToArray();


        /*foreach (var category in withLevelUp.Concat(withoutLevelUp))
        {
            category.transform.SetAsLastSibling();
        }
        */

        for (int i = 0; i < sortedCategories.Count; i++)
        {
            sortedCategories[i].transform.SetSiblingIndex(i);
        }

        CheckLevelUpAvailableCategories(encyclopediaType);
    }

    private void CacheCategoryInfo()
    {
        categoryCache.Clear();

        categoryCache[encyclopediaType] = new Dictionary<EncyclopediaCategory, (bool HasLevelUp, Rank Rank)>();

        var currentCategories = encyclopediaCategories[encyclopediaType];

        foreach (var category in currentCategories)
        {
            bool hasLevelUp = category.isLevelUpAvailable;
            Rank rank = category.rank;
            categoryCache[encyclopediaType][category] = (hasLevelUp, rank);
        }
    }

}
