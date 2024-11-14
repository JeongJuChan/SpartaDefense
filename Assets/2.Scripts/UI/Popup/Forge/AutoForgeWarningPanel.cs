using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoForgeWarningPanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private AutoForgeUIPanel autoForgeUIPanel;

    [SerializeField] private GameObject topPanel;

    [SerializeField] private Button otherDisableButton;

    public void Init()
    {
        cancelButton.onClick.AddListener(() => ActivateParentPopup(false));
        autoForgeUIPanel.OnShowWarningPopup += () => ActivateParentPopup(true);
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
        autoForgeUIPanel.ActivateAutoForge();
        ActivateParentPopup(false);
    }
}
