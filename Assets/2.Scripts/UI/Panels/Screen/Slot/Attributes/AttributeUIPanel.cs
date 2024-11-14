using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AttributeUIPanel : UIElementPooler<AttributeUIElement>, UIInitNeeded
{
    [SerializeField] private AttributeUIElement[] attributeUIElements;
    [SerializeField] private SlotUIPanel slotUIPanel;
    [SerializeField] private RectTransform referenceRect;

    private float referenceWidth;

    private AttributeType[] attributeTypeArr;

    public void Init()
    {
        attributeTypeArr = (AttributeType[])Enum.GetValues(typeof(AttributeType));
        int attributeLength = attributeTypeArr.Length;

        InitPool(attributeUIElements);

        slotUIPanel.OnUpdateSlotUIAttributes += UpdateAttributeUI;
        referenceWidth = referenceRect.sizeDelta.x;
    }

    protected override void InitPool(AttributeUIElement[] uiElements)
    {
        inactivePool = new Queue<AttributeUIElement>(uiElements.Length);
        activePool = new Queue<AttributeUIElement>(uiElements.Length);

        foreach (AttributeUIElement element in uiElements)
        {
            element.SetActive(false);
            element.SetWidth(referenceWidth);
            inactivePool.Enqueue(element);
        }
    }

    private void UpdateAttributeUI(Dictionary<int, float> attributeDict)
    {
        int activePoolCount = activePool.Count;
        int attributeCount = 0;

        if (attributeDict != null)
        {
            attributeCount = attributeDict.Count;
        }

        if (attributeDict != null)
        {
            foreach (int index in attributeDict.Keys)
            {
                if (activePoolCount > 0)
                {
                    SetAttributeInActivePool(ref activePool, attributeDict, index);
                    activePoolCount--;
                }
                else
                {
                    SetAttributeInActivePool(ref inactivePool, attributeDict, index);
                }
            }
        }

        DeActivateActiveElements(attributeCount);
    }

    private void SetAttributeInActivePool(ref Queue<AttributeUIElement> pool, Dictionary<int, float> attributeDict, int index)
    {
        AttributeUIElement element = pool.Dequeue();
        element.UpdateAttribute(EnumToKRManager.instance.GetEnumToKR(attributeTypeArr[index + 1]), attributeDict[index]);
        element.SetActive(true);
        activePool.Enqueue(element);
    }
}
