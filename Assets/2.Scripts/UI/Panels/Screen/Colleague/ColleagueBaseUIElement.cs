using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ColleagueBaseUIElement : MonoBehaviour
{
    [SerializeField] protected Image mainImage;
    [SerializeField] protected Button colleaguePopupButton;
    [SerializeField] protected TextMeshProUGUI levelText;
    [SerializeField] protected Button equipStateButton;
    [SerializeField] protected Image disablePanel;
    [SerializeField] protected GameObject lockIcon;

    [SerializeField] protected ColleagueUIData colleagueUIData;

    protected Sprite subtractSprite;

    public event Action<ColleagueUIData> OnOpenColleagueUIPopup;

    public virtual void Init()
    {
        colleaguePopupButton.onClick.AddListener(OpenColleagueUIPopup);
    }

    public void SetSubtractSprite(Sprite subtractSprite)
    {
        this.subtractSprite = subtractSprite;
    }

    public abstract void UpdateColleagueUIData(ColleagueUIData colleagueUIData);

    public ColleagueUIData GetColleagueUIData()
    {
        return colleagueUIData;
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = $"Lv.{level}";
    }

    private void OpenColleagueUIPopup()
    {
        if (colleagueUIData.index == -1)
        {
            return;
        }

        OnOpenColleagueUIPopup?.Invoke(colleagueUIData);
    }
}
