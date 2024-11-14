using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text effectText;
    [SerializeField] private Button lockButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite[] lockSprites; // 0: unlock, 1: lock
    [SerializeField] private Color[] lockColor; // 0: unlock, 1: lock 

    private int slotIndex;
    private Rank rank;
    private AbilityOptionEffectType effectType;
    private float effectValue;
    ArithmeticStatType arithmeticStatType;
    private bool isLocked;
    private bool isApplied = false;

    private BattleManager battleManager;
    private Dictionary<string, object> cachedData = new Dictionary<string, object>();

    public void AddCallbacks()
    {
        lockButton.onClick.AddListener(ClickLockButton);
    }

    public void InitSlot(int slotIndex, BattleManager battleManager)
    {
        this.slotIndex = slotIndex;
        this.battleManager = battleManager;


        if (LoadSlot())
        {
            CacheData();
            SetOptionUI();
            lockImage.sprite = lockSprites[isLocked ? 1 : 0];
            backgroundImage.color = lockColor[isLocked ? 1 : 0];


            ApplyAbility();
        }
        else
        {
            rank = Rank.None;
            effectType = AbilityOptionEffectType.None;
            effectValue = 1.0f;
            arithmeticStatType = ArithmeticStatType.Base;
        }
    }

    public void StartInitStat()
    {
        var statType = EnumUtility.ConvertToStatType(effectType);
        var attributeType = EnumUtility.ConvertToAttributeType(effectType);

        if (statType.HasValue)
        {
            AddStat(((int)effectValue), statType.Value);
        }
        else if (attributeType.HasValue)
        {
            AddAttributeStat(effectValue, attributeType.Value);
        }
    }

    public void SetSlot(Rank rank, AbilityOptionEffectType effectType, float effectValue, ArithmeticStatType arithmeticStatType)
    {
        if (isLocked) return;

        this.rank = rank;
        this.effectType = effectType;
        this.effectValue = effectValue;
        this.arithmeticStatType = arithmeticStatType;

        isApplied = false;

        SetOptionUI();
    }

    private void SetOptionUI()
    {
        if (rank == Rank.None || effectType == AbilityOptionEffectType.None)
        {
            rankText.text = "없음";
            effectText.text = "없음";
            return;
        }

        rankText.text = EnumToKRManager.instance.GetEnumToKR(rank);
        rankText.color = ResourceManager.instance.rank.GetRankColor(rank);

        // arithmeticStatType

        var statType = EnumUtility.ConvertToStatType(effectType);
        var attributeType = EnumUtility.ConvertToAttributeType(effectType);

        if (statType.HasValue)
        {
            effectText.text = EnumToKRManager.instance.GetStatTypeText(statType, effectValue);
        }
        else if (attributeType.HasValue)
        {
            effectText.text = EnumToKRManager.instance.GetAttributeTypeText(attributeType, arithmeticStatType, effectValue);
        }

    }

    private void ClickLockButton()
    {
        isLocked = !isLocked;
        lockImage.sprite = lockSprites[isLocked ? 1 : 0];
        backgroundImage.color = lockColor[isLocked ? 1 : 0];
    }

    public bool GetIsLocked()
    {
        return isLocked;
    }


    public void Save()
    {
        SaveSlot();
    }

    private void SaveSlot()
    {
        ES3.Save($"AbilitySlot_Rank_{slotIndex}", rank, ES3.settings);
        ES3.Save($"AbilitySlot_AbilityOptionEffectType_{slotIndex}", effectType, ES3.settings);
        ES3.Save($"AbilitySlot_EffectValue_{slotIndex}", effectValue, ES3.settings);
        ES3.Save($"AbilitySlot_ArithmeticStatType_{slotIndex}", arithmeticStatType, ES3.settings);
        ES3.Save($"AbilitySlot_IsLocked_{slotIndex}", isLocked, ES3.settings);

        ES3.StoreCachedFile();
    }

    private bool LoadSlot()
    {
        if (ES3.KeyExists($"AbilitySlot_Rank_{slotIndex}"))
        {
            rank = ES3.Load<Rank>($"AbilitySlot_Rank_{slotIndex}");
            effectType = ES3.Load<AbilityOptionEffectType>($"AbilitySlot_AbilityOptionEffectType_{slotIndex}");
            effectValue = ES3.Load<float>($"AbilitySlot_EffectValue_{slotIndex}");
            arithmeticStatType = ES3.Load<ArithmeticStatType>($"AbilitySlot_ArithmeticStatType_{slotIndex}");
            isLocked = ES3.Load<bool>($"AbilitySlot_IsLocked_{slotIndex}");

            return true;
        }

        return false;

    }

    private void CacheData()
    {
        cachedData["Rank"] = rank;
        cachedData["EffectType"] = effectType;
        cachedData["EffectValue"] = effectValue;
        cachedData["ArithmeticStatType"] = arithmeticStatType;
        cachedData["IsLocked"] = isLocked;
    }

    private void ClearCachedData()
    {
        // 이전 능력치 적용
        if (cachedData.Count == 0) return;

        var statType = EnumUtility.ConvertToStatType(cachedData["EffectType"] as AbilityOptionEffectType?);
        var attributeType = EnumUtility.ConvertToAttributeType(cachedData["EffectType"] as AbilityOptionEffectType?);

        if (statType.HasValue)
        {
            RemoveStat(((int)((float)cachedData["EffectValue"])), statType.Value);
        }
        else if (attributeType.HasValue)
        {
            RemoveAttributeStat((float)cachedData["EffectValue"], attributeType.Value);
        }

        cachedData.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ApplyAbility();
        }
    }

    public void ApplyAbility()
    {
        if (isApplied) return;

        if (cachedData.Count != 0)
        {
            ClearCachedData();
        }

        CacheData();


        var statType = EnumUtility.ConvertToStatType(effectType);
        var attributeType = EnumUtility.ConvertToAttributeType(effectType);

        if (statType.HasValue)
        {
            AddStat(((int)effectValue), statType.Value);
        }
        else if (attributeType.HasValue)
        {
            AddAttributeStat(effectValue, attributeType.Value);
        }

        isApplied = true;
    }

    private void AddStat(float currentValue, StatType currentStatType)
    {
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Ability, currentStatType, currentValue, true);
    }

    private void RemoveStat(float beforeValue, StatType beforeStatType)
    {
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Ability, beforeStatType, beforeValue, false);
    }

    private void AddAttributeStat(float currentValue, AttributeType currentAttributeType)
    {
        StatDataHandler.Instance.AddAttributeStat(currentAttributeType, currentValue);
    }

    private void RemoveAttributeStat(float beforeValue, AttributeType beforeAttributeType)
    {
        StatDataHandler.Instance.RemoveAttributeStat(beforeAttributeType, beforeValue);
    }

    private void OnDestroy()
    {
        SaveSlot();
    }

    // arithmeticStatType이 statType인지, attributeType인지 구분하는 메서드


}
