using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankDataHandler
{
    private Sprite[] colorSprites;
    private Sprite[] colorSpritesForText;
    private Dictionary<Rank, Sprite> rankBackgroundSpriteDict = new Dictionary<Rank, Sprite>();
    private Dictionary<Rank, Color> rankColorDict = new Dictionary<Rank, Color>();

    public RankDataHandler(Sprite[] colorSprites, Sprite[] colorSpritesForText, NewSlotUIPanel newSlotUIPanel)
    {
        this.colorSprites = colorSprites;
        this.colorSpritesForText = colorSpritesForText;
        InitRankSprites();
        newSlotUIPanel.OnGetRankBackgroundSprite += GetRankBackgroundSprite;
        newSlotUIPanel.OnGetRankColor += GetRankColor;
        //skillEquipUIController.OnGetRankBackgroundSprite += GetRankBackgroundSprite;
        //skillEquipUIController.OnGetRankColor += GetRankColor;
    }

    public Sprite GetRankBackgroundSprite(Rank rank)
    {
        if (rankBackgroundSpriteDict == null)
        {
            InitRankSprites();
        }
        return rankBackgroundSpriteDict[rank];
    }

    public Color GetRankColor(Rank rank)
    {
        return rankColorDict[rank];
    }

    private void InitRankSprites()
    {
        for (int i = 0; i < colorSpritesForText.Length; i++)
        {
            Rank rank = (Rank)(i + 1);
            rankBackgroundSpriteDict.Add(rank, colorSprites[i]);

            Color color = colorSpritesForText[i].texture.GetPixel(
                colorSpritesForText[i].texture.width / 2, colorSpritesForText[i].texture.height / 2);
            rankColorDict.Add(rank, color);
        }
    }
}