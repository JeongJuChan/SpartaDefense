using System;
using UnityEngine;

public class SkillTopUIElement : SkillUIElement
{
    public event Action<int, SkillTopUIElement> OnEquipNewSkill;

    private Sprite offsetBackgroundSprite;
    [SerializeField] private GameObject levelBar;
    [SerializeField] private GameObject mainSpriteMask;

    protected override void Awake()
    {
        backgroundButton.onClick.AddListener(ShowSkillInfoPopup);
        offsetBackgroundSprite = backgroundButton.image.sprite;
        ActiveLevelPanel(false);
        //mainSpriteMask.gameObject.SetActive(false);
    }

    public override void OnSlotOpened()
    {
        base.OnSlotOpened();
        backgroundButton.image.sprite = offsetBackgroundSprite;
        mainSpriteMask.gameObject.SetActive(true);
        ActiveLevelPanel(false);
    }


    public void EnableEquipButton()
    {
        SwapEvent(false);
        backgroundButton.enabled = true;
    }

    public void DisableEquipButton()
    {
        backgroundButton.enabled = false;
    }

    public void ActiveLevelPanel(bool isActive)
    {
        levelText.gameObject.SetActive(isActive);
        levelBar.gameObject.SetActive(isActive);
    }

    public void SwapEvent(bool isActivateSwtichEvent)
    {
        if (isActivateSwtichEvent)
        {
            backgroundButton.onClick.RemoveListener(ShowSkillInfoPopup);
            backgroundButton.onClick.AddListener(SwitchSkill);
        }
        else
        {
            backgroundButton.onClick.RemoveListener(SwitchSkill);
            backgroundButton.onClick.AddListener(ShowSkillInfoPopup);
        }
    }

    private void SwitchSkill()
    {
        int unEquipIndex = GetIndex();
        equipStateButton.onClick.Invoke();
        OnEquipNewSkill?.Invoke(unEquipIndex, this);
        ChangeActiveStateMainSprite(true);
        ActiveLevelPanel(true);
        SwapEvent(false);
    }
}
