using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColleagueTopUIElement : ColleagueBaseUIElement
{
    [SerializeField] private GameObject levelBar;
    [SerializeField] private GameObject mainSpriteMask;
    [SerializeField] private GameObject equipIcon;

    private Sprite backgroundOffsetSprite;

    // Init Settings for this class
    public void UpdateUnEquipState(Action onClick)
    {
        backgroundOffsetSprite = colleaguePopupButton.image.sprite;
        equipStateButton.onClick.AddListener(() => onClick.Invoke());
    }

    public override void UpdateColleagueUIData(ColleagueUIData colleagueUIData)
    {
        if (colleagueUIData.colleagueType == ColleagueType.Rtan_Rare)
        {
            ResetColleagueUI();
        }
        else
        {
            equipStateButton.gameObject.SetActive(true);
        }

        this.colleagueUIData = colleagueUIData;
        mainImage.sprite = colleagueUIData.mainSprite;
        colleaguePopupButton.image.sprite = colleagueUIData.backgroundSprite;
        equipStateButton.image.sprite = subtractSprite;
        levelBar.SetActive(true);
        levelText.gameObject.SetActive(true);
        colleaguePopupButton.gameObject.SetActive(true);
        equipIcon.SetActive(false);
        mainImage.gameObject.SetActive(true);
    }

    public void ResetColleagueUI()
    {
        colleagueUIData.index = -1;
        lockIcon.SetActive(false);
        if (backgroundOffsetSprite != null)
        {
            colleaguePopupButton.image.sprite = backgroundOffsetSprite;
        }
        levelText.gameObject.SetActive(false);
        levelBar.SetActive(false);
        equipStateButton.gameObject.SetActive(false);
        mainImage.gameObject.SetActive(false);
        equipIcon.SetActive(true);
        disablePanel.gameObject.SetActive(false);
    }

    public int GetIndex()
    {
        return colleagueUIData.index;
    }
}
