using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillAutoButtonsUIPanel : MonoBehaviour
{
    [SerializeField] private Button autoEquipButton;
    [SerializeField] private Button autoEnhanceButton;

    public event Action OnEquipSkillsAuto;
    public event Action OnEnhanceSkillsAuto;

    public event Func<bool> OnGetAutoEquipState;
    public event Func<bool> OnGetAutoEnhanceState;

    private void Awake()
    {
        autoEquipButton.onClick.AddListener(EquipSkillsAuto);
        autoEnhanceButton.onClick.AddListener(EnhanceSkillsAuto);
    }

    private void OnEnable()
    {
        if (GameManager.instance.isInitializing)
        {
            if (OnGetAutoEquipState.Invoke())
            {
                autoEquipButton.interactable = true;
            }
            else
            {
                autoEquipButton.interactable = false;

            }

            if (OnGetAutoEnhanceState.Invoke())
            {
                autoEnhanceButton.interactable = true;
            }
            else
            {
                autoEnhanceButton.interactable = false;
            }
        }
    }

    private void EquipSkillsAuto()
    {
        OnEquipSkillsAuto?.Invoke();

        if (OnGetAutoEquipState.Invoke())
        {
            UpdateActiveEquipState(true);
        }
        else
        {
            UpdateActiveEquipState(false);
        }
    }

    public void UpdateActiveEquipState(bool isActive)
    {
        autoEquipButton.interactable = isActive;
    }

    private void EnhanceSkillsAuto()
    {
        OnEnhanceSkillsAuto?.Invoke();

        if (OnGetAutoEnhanceState.Invoke())
        {
            UpdateActiveEnhanceState(true);
        }
        else
        {
            UpdateActiveEnhanceState(false);
        }
    }

    public void UpdateActiveEnhanceState(bool isActive)
    {
        autoEnhanceButton.interactable = isActive;
    }
}
