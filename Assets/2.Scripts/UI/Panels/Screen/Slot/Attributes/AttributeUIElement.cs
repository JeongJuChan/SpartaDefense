using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttributeUIElement : MonoBehaviour, IUIElement
{
    [SerializeField] private TextMeshProUGUI attributeText;
    [SerializeField] private TextMeshProUGUI attributeStatText;

    public void UpdateAttribute(string statType, float stat)
    {
        attributeText.text = statType;
        attributeStatText.text = $"{stat}%";
    }

    public void SetWidth(float width)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size.x = width;
        rectTransform.sizeDelta = size;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
