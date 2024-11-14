using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;

public class SlotDetailAttributePanel : MonoBehaviour, IUIElement
{
    [SerializeField] private TextMeshProUGUI attributeTitleText;
    [SerializeField] private TextMeshProUGUI attributeText;


    public void SetActive(bool isActive)
    {
        attributeTitleText.gameObject.SetActive(isActive);
        attributeText.gameObject.SetActive(isActive);
    }

    public void UpdateAttributeText(string attributeKR, float percent)
    {
        this.attributeTitleText.text = attributeKR;
        attributeText.text = $"{percent:F2}%";
    }

    public void UpdateAttributeText(string attributeKR, BigInteger value)
    {
        this.attributeTitleText.text = attributeKR;
        value = value < 0 ? -value : value;
        attributeText.text = value.ToString();
    }
}
