using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncyclopediaSlot : MonoBehaviour
{
    // 데이터가 들어있는지 확인하는 bool
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private GameObject hideObject;
    [SerializeField] private TextMeshProUGUI levelText;
    public int level { get; private set; }
    public bool isDataEmpty { get; private set; } = true;
    private string skillRealName;
    private EncyclopediaType encyclopediaType;
    private ColleagueEncyclopediaType colleagueEncyclopediaType;

    private int goalLevel;

    public void Init()
    {
        isDataEmpty = true;
    }

    public void SetData(EncyclopediaSlotData data, int categoryLevel, EncyclopediaType encyclopediaType)
    {
        /*this.encyclopediaType = encyclopediaType;
        switch (encyclopediaType)
        {
            case EncyclopediaType.Equipment:
                icon.sprite = EquipmentManager.instance.GetIcon(data.itemName);
                background.color = ResourceManager.instance.rank.GetRankColor(Enum.Parse<Rank>(data.itemName.Split('_')[1]));
                break;
            case EncyclopediaType.Skill:
                var temp = data.itemName.Split('_');
                skillRealName = temp[0];
                icon.sprite = ResourceManager.instance.skill.GetSkillResourceData(int.Parse(temp[0])).skillIcon;
                background.color = ResourceManager.instance.rank.GetRankColor(Enum.Parse<Rank>(temp[1]));
                level = ES3.Load<int>(skillRealName + "_Level", 0);
                break;
        }

        levelText.text = $"{level}/{categoryLevel + 1}";

        hideObject.SetActive(level < (categoryLevel + 1));

        isDataEmpty = false;*/
    }

    public void SetData(ColleagueInfo colleagueInfo, int goalLevel, EncyclopediaType encyclopediaType, 
        ColleagueEncyclopediaType colleagueEncyclopediaType)
    {
        this.goalLevel = goalLevel;
        this.encyclopediaType = encyclopediaType;
        this.colleagueEncyclopediaType = colleagueEncyclopediaType;

        icon.sprite = ResourceManager.instance.slotHeroData.GetResource(colleagueInfo).defaultSprite;
        background.sprite = ResourceManager.instance.rank.GetRankBackgroundSprite(colleagueInfo.rank);

        levelText.text = $"{level}/{goalLevel}";

        hideObject.SetActive(level < goalLevel);

        isDataEmpty = false;
    }

    public void UpdateGoalLevel(int goalLevel)
    {
        this.goalLevel = goalLevel;
        UpdateLevelText();
    }

    public void SetMaxLevelText()
    {
        levelText.text = level.ToString();
    }

    public ColleagueEncyclopediaType GetColleagueEncyclopediaType()
    {
        return colleagueEncyclopediaType;
    }

    public void UpdateLevel(int level)
    {
        this.level = level;
        UpdateLevelText();
    }

    public bool GetIsLevelUpAvailable()
    {
        return level >= goalLevel;
    }

    public void UpdateLevelText()
    {
        levelText.text = $"{level}/{goalLevel}";
        hideObject.SetActive(level < (goalLevel));
    }

    public void UpdateLevel(EncyclopediaSlotData data)
    {
        switch (encyclopediaType)
        {
            case EncyclopediaType.Equipment:
                level = ES3.Load<int>(data.itemName + "_Level", 0);
                return;
            case EncyclopediaType.Skill:
                level = ES3.Load<int>(skillRealName + "_Level", 0);
                return;
        }
    }
}
