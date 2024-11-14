using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotHeroDataHandler
{
    private Dictionary<ColleagueInfo, SlotHeroData> slotHeroDict = new Dictionary<ColleagueInfo, SlotHeroData>();
    private Dictionary<ColleagueType, Sprite> slotLockSpriteDict = new Dictionary<ColleagueType, Sprite>();

    private const string SLOT_PATH = "Characters/Heroes/Slots";
    private const string RTAN_PATH = "Characters/Heroes/RtanSprite";
    private const string UI_SLOT_PATH = "UI/MainSlotIcons/";

    private Rank[] ranks;

    private List<Sprite> sprites = new List<Sprite>();
    private List<RuntimeAnimatorController> runtimeAnimatorControllers = new List<RuntimeAnimatorController>();


    public SlotHeroDataHandler()
    {
        ranks = (Rank[])Enum.GetValues(typeof(Rank));
    }

    public SlotHeroData GetResource(ColleagueInfo colleagueInfo)
    {
        if (!slotHeroDict.ContainsKey(colleagueInfo))
        {
            LoadSlotTypeProjectiles(colleagueInfo.colleagueType);
        }

        return slotHeroDict[colleagueInfo];
    }

    public Sprite GetSlotLockSprite(ColleagueType slotType)
    {
        if (slotLockSpriteDict.ContainsKey(slotType))
        {
            return slotLockSpriteDict[slotType];
        }

        return default;
    }

    public void LoadSlotTypeProjectiles(ColleagueType slotType)
    {
        sprites.Clear();
        runtimeAnimatorControllers.Clear();

        string middlePath = slotType.ToString().Split('_')[0];

        for (int i = 1; i < ranks.Length; i++)
        {
            string path = $"{SLOT_PATH}/{middlePath}/{slotType}";
            sprites.Add(Resources.Load<Sprite>(path));
            runtimeAnimatorControllers.Add(Resources.Load<RuntimeAnimatorController>(path));
        }

        SlotHeroData[] slotHeroDatas = new SlotHeroData[sprites.Count];

        /*if (slotType == ColleagueType.Rtan)
        {
            for (int i = 0; i < slotHeroDatas.Length; i++)
            {
                ColleagueInfo slotInfo = new ColleagueInfo(ranks[i + 1], slotType);
                slotHeroDatas[i] = new SlotHeroData(slotInfo, sprites[i], runtimeAnimatorControllers[0]);

                if (!slotHeroDict.ContainsKey(slotInfo))
                {
                    slotHeroDict.Add(slotInfo, slotHeroDatas[i]);
                }
            }
        }
        else*/
        {
            for (int i = 0; i < slotHeroDatas.Length; i++)
            {
                ColleagueInfo slotInfo = new ColleagueInfo(ranks[i + 1], slotType);
                slotHeroDatas[i] = new SlotHeroData(slotInfo, sprites[i], runtimeAnimatorControllers[i]);

                if (!slotHeroDict.ContainsKey(slotInfo))
                {
                    slotHeroDict.Add(slotInfo, slotHeroDatas[i]);
                }
            }
        }
    }
}