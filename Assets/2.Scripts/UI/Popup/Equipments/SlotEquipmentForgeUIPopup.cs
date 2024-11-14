using Keiwando.BigInteger;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotEquipmentForgeUIPopup : UI_Base
{
    [SerializeField] private RectTransform comparisonPanel;
    [SerializeField] private Button touchBackButton;
    [SerializeField] private ForgeUIButton forgeUIButton;
    [SerializeField] private SlotDecisionUIPanel slotDescisionUIPanel;
    private const float COMPARISION_PANEL_POSY = 0.1f;

    [SerializeField] private AutoForgeUIPanel autoForgeUIPanel;
    
    public event Action<EquipmentType, int, BigInteger, BigInteger> OnSellNewSlotDelay;

    private Action OnActivateAutoForge;

    private Action OnDeActiveDisablePanel;

    private bool isNewSlotItemEmpty = true;

    private event Action OnButtonActive;
    private Action OnSelfActivated;


    public void Init()
    {
        Vector2 comparisonPos = comparisonPanel.anchoredPosition;
        comparisonPos.y = COMPARISION_PANEL_POSY * Screen.height;
        comparisonPanel.anchoredPosition = comparisonPos;

        DeActivateSelf(true);

        if (forgeUIButton == null)
        {
            forgeUIButton = FindAnyObjectByType<ForgeUIButton>();
        }

        forgeUIButton.OnActiveSlotUIPopup += () => ActiveSelf(true);

        OnActivateAutoForge += autoForgeUIPanel.TryActivateAutoForge;
        SlotEquipmentForger slotEquipmentForger = FindAnyObjectByType<SlotEquipmentForger>();

        slotEquipmentForger.OnActiveForgePopup += ActivateSelf;

        ForgePreImagePanel forgePreImagePanel = UIManager.instance.GetUIElement<ForgePreImagePanel>();

        slotEquipmentForger.OnSellNewSlotDelay += SellDelay;

        touchBackButton.onClick.AddListener(OnPointerDown);

        forgeUIButton.OnGetIsSlotEmpty += GetIsSlotEmpty;

        forgeUIButton.OnDeActivateAutoForgeDeactiveButton += autoForgeUIPanel.DeActiveDisableButton;
        forgeUIButton.OnActivateAutoForgeDeActiveButton += autoForgeUIPanel.ActivateDisableButton;

        OnDeActiveDisablePanel += autoForgeUIPanel.DeActiveDisableButton;
        OnDeActiveDisablePanel += forgeUIButton.DeActiveDisableButton;

        OnButtonActive += forgeUIButton.SetOnForgeButton;

        forgePreImagePanel.OnClickPreImageButton += () => ActiveSelf(true);
        OnSelfActivated += () => forgePreImagePanel.ActivatePreImageButton(true);

        autoForgeUIPanel.OnShowUIPopup += ActiveSelf;
    }

    public void DeActivateSelf(bool isSlotEmpty)
    {
        this.isNewSlotItemEmpty = isSlotEmpty;
        forgeUIButton.forgeButton.enabled = true;
        ActiveSelf(false);
    }

    public void ActivateSelf(bool isAuto)
    {
        forgeUIButton.StartCoroutine(CoWaitForActive(isAuto));
    }

    private void SellDelay(EquipmentType equipmentType, int level, BigInteger originExp, BigInteger originGold)
    {
        forgeUIButton.StartCoroutine(CoWaitForSellDelay(equipmentType, level, originExp, originGold));
    }

    private IEnumerator CoWaitForSellDelay(EquipmentType equipmentType, int level, BigInteger originExp, BigInteger originGold)
    {
        if (!isNewSlotItemEmpty)
        {
            ActiveSelf(true);
            yield break;
        }

        OnSellNewSlotDelay?.Invoke(equipmentType, level, originExp, originGold);

        slotDescisionUIPanel.OnClickSellingButton -= autoForgeUIPanel.TryActivateAutoForge;

        forgeUIButton.EndForge(false);
        OnActivateAutoForge?.Invoke();
    }

    private IEnumerator CoWaitForActive(bool isAuto)
    {
        if (!isNewSlotItemEmpty)
        {
            ActiveSelf(true);
            yield break;
        }

        slotDescisionUIPanel.OnClickSellingButton -= autoForgeUIPanel.TryActivateAutoForge;

        if (isAuto)
        {
            forgeUIButton.EndForge(false);
            OnDeActiveDisablePanel?.Invoke();

            if (!UIManager.instance.isPopupOpened)
            {
                ActiveSelf(true);
            }
            else
            {
                OnSelfActivated();
                OnDeActiveDisablePanel?.Invoke();
                OnButtonActive?.Invoke();
            }

            slotDescisionUIPanel.OnClickSellingButton += autoForgeUIPanel.TryActivateAutoForge;
        }
        else
        {
            ActiveSelf(true);
            OnDeActiveDisablePanel?.Invoke();
            forgeUIButton.EndForge(true);
            //DialogManager.instance.ShowDialog(DialogueType.AcquireEquipment);
        }

        isNewSlotItemEmpty = false;
    }

    private void OnPointerDown()
    {
        DeActivateSelf(false);
    }

    private bool GetIsSlotEmpty()
    {
        return isNewSlotItemEmpty;
    }

    private void ActiveSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
        OnSelfActivated?.Invoke();
    }
}
