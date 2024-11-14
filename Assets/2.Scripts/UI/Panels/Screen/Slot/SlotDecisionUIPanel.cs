using System;
using UnityEngine;
using UnityEngine.UI;

public class SlotDecisionUIPanel : UI_Base, UIInitNeeded, IUIElement
{
    [SerializeField] private Button sellButton;
    [SerializeField] private Button EquipButton;

    [SerializeField] private CurrentSlotUIPanel currentSlotUIPanel;
    [SerializeField] private NewSlotUIPanel newSlotUIPanel;

    public event Action OnClickSellingButton;

    private GameObject sellButtonPanel;

    public void Init()
    {
        sellButton.onClick.AddListener(() => OnClickSellingButton?.Invoke());
        EquipButton.onClick.AddListener(ForgeManager.instance.SwapSlotPanels);
        sellButtonPanel = sellButton.transform.parent.gameObject;
        currentSlotUIPanel.OnActiveSellButton += SetActive;
    }

    public void SetActive(bool isActive)
    {
        sellButtonPanel.SetActive(isActive);

        if (isActive)
        {
            //     sellButtonPanel.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            // DialogManager.instance.ShowDialog(sellButtonPanel.transform, DialogueType.SellItems);
            //DialogManager.instance.ShowDialog(DialogueType.SellItems);
        }


    }
}
