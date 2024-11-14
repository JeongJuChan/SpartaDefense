using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSlotElement : MonoBehaviour
{
    [SerializeField] private Image rankBackgroundImage;
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI slotText;

    [SerializeField] private Button mainSlotButton;
    [SerializeField] private GameObject outFrame;

    private Sprite defaultSprite;

    private EquipmentType equipmentType = EquipmentType.None;

    public event Action<EquipmentType, FeatureType, int> OnOpenSlotDetail;

    [SerializeField] private int elementIndex;

    private FeatureType featureType;
    private int unlockCount;

    private void Awake()
    {
        mainSlotButton.onClick.AddListener(TryOpenSlotDetail);
    }

    public void Init(int index)
    {
        elementIndex = index;
    }

    public void InitUnlockData(string defaultName)
    {
        UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(
            EnumUtility.GetEqualValue<FeatureID>($"{defaultName}{elementIndex + 1}"));
        featureType = unlockData.featureType;
        unlockCount = unlockData.count;
    }

    public void SetUnlockValues(FeatureType featureType, int unlockCount)
    {
        this.featureType = featureType;
        this.unlockCount = unlockCount;
    }

    private void TryOpenSlotDetail()
    {
        OnOpenSlotDetail?.Invoke(equipmentType, featureType, unlockCount);
    }

    public void UpdateSlotUIElement(EquipmentType equipmentType, Sprite rankSpirte, Sprite slotSprite, int level)
    {
        this.equipmentType = equipmentType;

        if (defaultSprite == null)
        {
            defaultSprite = rankBackgroundImage.sprite;
        }
        
        if (slotSprite == null)
        {
            slotSprite = defaultSprite;
        }
        else if (outFrame.activeInHierarchy)
        {
            outFrame.SetActive(false);
        }

        if (slotImage.color.a < 1f)
        {
            Color color = slotImage.color;
            color.a = 1f;
            slotImage.color = color;
        }

        rankBackgroundImage.sprite = rankSpirte;
        slotImage.sprite = slotSprite;
        slotText.text = $"LV. {level}";
    }

    public void InitSprite(Sprite sprite)
    {
        slotImage.sprite = sprite;
    }

    public Sprite GetRankDefaultSprite()
    {
        return rankBackgroundImage.sprite;
    }

    public void TrySetDefaultImage(Sprite sprite)
    {
        slotImage.sprite = sprite;
    }
}
