using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotDetailStatPanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private SlotDetailPanel slotDetailPanel;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI defenseText;

    [SerializeField] private SlotDetailAttributePanel[] slotDetailAttributePanels;

    [SerializeField] private string[] attributetypesKR;

    private void UpdateDefaultStatUI(BigInteger hp, BigInteger damage, BigInteger defense)
    {
        hpText.text = hp.ToString();
        damageText.text = damage.ToString();
        defenseText.text = defense.ToString();
    }

    private void OnUpdateDetailAttributePanels(Dictionary<int, float> attributeDict)
    {
        foreach (SlotDetailAttributePanel slotDetailAttributePanel in slotDetailAttributePanels)
        {
            slotDetailAttributePanel.SetActive(false);
        }

        int index = 0;

        foreach (var item in attributeDict)
        {
            slotDetailAttributePanels[index].SetActive(true);
            slotDetailAttributePanels[index].UpdateAttributeText(attributetypesKR[item.Key], item.Value);
            index++;
        }
    }

    public void Init()
    {
        AttributeType[] attributeTypes = (AttributeType[])Enum.GetValues(typeof(AttributeType));

        attributetypesKR = new string[attributeTypes.Length - 1];
         
        for (int i = 0; i < slotDetailAttributePanels.Length; i++)
        {
            attributetypesKR[i] = EnumToKRManager.instance.GetEnumToKR(attributeTypes[i + 1]);
            slotDetailAttributePanels[i].SetActive(false);
        }

        slotDetailPanel.OnUpdateSlotDefaultStatUI += UpdateDefaultStatUI;
        slotDetailPanel.OnUpdateSlotDetailAttributePanel += OnUpdateDetailAttributePanels;
    }
}
