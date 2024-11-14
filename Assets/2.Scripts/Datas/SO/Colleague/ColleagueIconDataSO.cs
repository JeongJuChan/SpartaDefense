using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueIconData", menuName = "SO/Colleague/ColleagueIconData")]
public class ColleagueIconDataSO : ScriptableObject
{
    [SerializeField] private ColleagueIconData[] colleagueIconDatas;
    private Dictionary<ColleagueType, ColleagueIconData> colleagueIconDataDict = new Dictionary<ColleagueType, ColleagueIconData>();

    private Sprite normalStarSprite;
    private Sprite advancedStarSprite;

    private const string DEFAULT_PATH = "Sprites/Icons/";

    public Sprite GetColleagueIcon(ColleagueType colleagueType)
    {
        if (colleagueIconDataDict.Count == 0)
        {
            InitDict();
        }

        return colleagueIconDataDict[colleagueType].colleagueSprite;
    }

    public void InitDict()
    {
        foreach (ColleagueIconData colleagueIconData in colleagueIconDatas)
        {
            if (!colleagueIconDataDict.ContainsKey(colleagueIconData.colleagueType))
            {
                colleagueIconDataDict.Add(colleagueIconData.colleagueType, colleagueIconData);
            }
        } 
    }

    public void Init()
    {
        normalStarSprite = Resources.Load<Sprite>($"{DEFAULT_PATH}ItemIcon_Star_Gold");
        advancedStarSprite = Resources.Load<Sprite>($"{DEFAULT_PATH}ItemIcon_Star_Purple");
    }

    public Sprite GetNormalStar()
    {
        return normalStarSprite;
    }

    public Sprite GetAdvancedStar()
    {
        return advancedStarSprite;
    }
    
}
