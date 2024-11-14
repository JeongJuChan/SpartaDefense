using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumToKRManager : MonoBehaviorSingleton<EnumToKRManager>
{
    private EnumToKRSO enumToKRSO;

    public void Init()
    {
        this.enumToKRSO = ResourceManager.instance.enumToKRSO;
    }

    public string GetStatTypeText(StatType? statType, float effectValue)
    {
        string statTypeKR = GetEnumToKR(statType);
        string afterText = $"{(int)effectValue} 증가";
        string resultStr = $"{statTypeKR} {afterText}";
        return resultStr;
    }

    public string GetAttributeTypeText(AttributeType? attributeType, ArithmeticStatType arithmeticStatType, float effectValue)
    {
        string attributeTypeKR = GetEnumToKR(attributeType);

        string afterText = "";

        switch (arithmeticStatType)
        {
            case ArithmeticStatType.Base:
                afterText = $"{effectValue:F2} 증가";
                break;
            case ArithmeticStatType.Rate:
                afterText = $"{effectValue:F2}% 증가";
                break;
        }

        string resultStr = $"{attributeTypeKR} {afterText}";
        return resultStr;
    }

    public string GetEnumToKR(Enum enumType)
    {
        Type type = enumType.GetType();
        string krStr = enumToKRSO.GetEnumToKRByType(type, Convert.ToInt32(enumType));
        return krStr;
    }
}