using System;
using System.Collections.Generic;
using System.Reflection;

public class AttributeDataHandler
{
    private AttributeAppliedData attributeAppliedData;
    private Dictionary<AttributeType, int> rtanAttributeIndexDict = new Dictionary<AttributeType, int>();
    private UI_Alert uI_Alert;

    private Dictionary<AttributeType, Action<float>> attributeEventDict = new Dictionary<AttributeType, Action<float>>();
    public event Action<AttributeType, float> OnUpdateAttributetext;

    public AttributeDataHandler()
    {

        /*ForgeManager.instance.OnAddAttributes += AddAttributeStats;
        ForgeManager.instance.OnRemoveAttributes += RemoveAttributeStats;*/

        //battleManager.OnGetAttributeAppliedValue += GetRtanAttributeValue;
        //castle.OnUpdateHealingAmount += GetRtanAttributeValue;

        /*rtan.OnGetAttributeValue += GetRtanAttributeValue;
        rtan.OnGetAttributeIndexDict += GetRtanAttributeIndexDict;
        rtan.OnGetAttributeIndex += GetRtanAttributeIndex;
        rtan.InitRtanAttributeDict();*/

        InitAttributeIndexDict();

        attributeAppliedData = new AttributeAppliedData();
        AttributeType[] attributeTypes = (AttributeType[])Enum.GetValues(typeof(AttributeType));
        attributeAppliedData.attributeAppliedStat = new float[attributeTypes.Length - 1];

        uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();
    }

    public float GetAttributeAppliedData(AttributeType attributeType)
    {
        return attributeAppliedData.attributeAppliedStat[(int)attributeType - 1];
    }

    private float GetRtanAttributeValue(int index)
    {
        return attributeAppliedData.attributeAppliedStat[index];
    }

    private float GetRtanAttributeValue(AttributeType attributeType)
    {
        return attributeAppliedData.attributeAppliedStat[rtanAttributeIndexDict[attributeType]];
    }

    private Dictionary<AttributeType, int> GetRtanAttributeIndexDict()
    {
        return rtanAttributeIndexDict;
    }

    private int GetRtanAttributeIndex(AttributeType attributeType)
    {
        return rtanAttributeIndexDict[attributeType];
    }

    public void AddAttributeStat(AttributeType attributeType, float upgradeValue)
    {
        int index = (int)attributeType - 1;
        attributeAppliedData.attributeAppliedStat[index] += upgradeValue;

        if (attributeEventDict.ContainsKey(attributeType))
        {
            attributeEventDict[attributeType]?.Invoke(attributeAppliedData.attributeAppliedStat[index]);
        }

        OnUpdateAttributetext?.Invoke(attributeType, attributeAppliedData.attributeAppliedStat[index]);

        uI_Alert.PowerMessage(StatViewerHelper.instance.GetBattlePowerChange());
    }

    public void RemoveAttributeStat(AttributeType attributeType, float upgradeValue)
    {
        int index = (int)attributeType - 1;

        attributeAppliedData.attributeAppliedStat[index] -= upgradeValue;

        if (attributeEventDict.ContainsKey(attributeType))
        {
            attributeEventDict[attributeType]?.Invoke(attributeAppliedData.attributeAppliedStat[index]);
        }

        OnUpdateAttributetext?.Invoke(attributeType, attributeAppliedData.attributeAppliedStat[index]);
    }

    private void AddAttributeStats(Dictionary<int, float> attributeStatDict)
    {
        if (attributeStatDict == null)
        {
            return;
        }

        foreach (int index in attributeStatDict.Keys)
        {
            attributeAppliedData.attributeAppliedStat[index] += attributeStatDict[index];
        }

        uI_Alert.PowerMessage(StatViewerHelper.instance.GetBattlePowerChange());
    }

    private void RemoveAttributeStats(Dictionary<int, float> attributeStatDict)
    {
        if (attributeStatDict == null)
        {
            return;
        }

        foreach (int index in attributeStatDict.Keys)
        {
            attributeAppliedData.attributeAppliedStat[index] -= attributeStatDict[index];
        }
    }

    public void AddAttributeEvent(AttributeType attributeType, Action<float> OnUpdateAttiributeAciton)
    {
        if (!attributeEventDict.ContainsKey(attributeType))
        {
            attributeEventDict.Add(attributeType, null);
        }

        attributeEventDict[attributeType] += OnUpdateAttiributeAciton;
    }

    public void RemoveAttributeEvent(AttributeType attributeType, Action<float> OnUpdateAttiributeAciton)
    {
        if (!attributeEventDict.ContainsKey(attributeType))
        {
            attributeEventDict.Add(attributeType, null);
        }

        attributeEventDict[attributeType] -= OnUpdateAttiributeAciton;
    }

    public void RemoveAttributeEvent(AttributeType attributeType)
    {
        if (attributeEventDict.ContainsKey(attributeType))
        {
            attributeEventDict.Remove(attributeType);
        }
    }

    private void InitAttributeIndexDict()
    {
        AttributeType[] attributeTypes = (AttributeType[])Enum.GetValues(typeof(AttributeType));

        for (int i = 1; i < attributeTypes.Length; i++)
        {
            rtanAttributeIndexDict.Add(attributeTypes[i], i - 1);
        }
    }
}