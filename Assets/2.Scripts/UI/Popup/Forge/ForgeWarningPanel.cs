using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeWarningPanel : UI_Base, UIInitNeeded
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private GameObject topPanel;

    [SerializeField] private Button otherDisableButton;

    public void Init()
    {
        cancelButton.onClick.AddListener(() => ActivateParentPopup(false));
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        otherDisableButton.onClick.AddListener(() => ActivateParentPopup(false));
        ActivateParentPopup(false);
    }

    public void ActivateParentPopup(bool isActive)
    {
        topPanel.SetActive(isActive);
    }

    private void OnClickConfirmButton()
    {
        ForgeManager.instance.SellNewSlot(true);
        ActivateParentPopup(false);
    }
}
