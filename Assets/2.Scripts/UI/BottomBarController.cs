using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BottomBarController : MonoBehaviour, UIInitNeeded
{
    [System.Serializable]
    public struct UIElement
    {
        public RedDotIDType redDotID;
        public FeatureID featureID;
        [HideInInspector] public UnlockData unlockData;
        public UI_BottomElement canvas;
        public Button button;
        public Image image;
        public TextMeshProUGUI text;
        public GameObject lockIcon;
    }

    [Header("UI Elements")]
    [SerializeField] private UIElement[] uiElements;
    [SerializeField] private UI_Base[] ui_bases;

    [Header("Icons")]
    [SerializeField] private Sprite closeSprite;

    [SerializeField] private GuideController guide;
    private bool[] isLocked = new bool[6] { false, true, true, true, true, true };

    private UIElement? activeElement = null;
    private Sprite activeSprite;
    private string activeElementName;
    private UI_Alert uI_Alert;

    public void Init()
    {
        LoadDatas();

        InitializeButtons();

        guide.Initialize();

        uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();

        StageDefeatPanel.OnGameFaileded += GameFailedCanvas;
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            int index = i;
            var uiElement = uiElements[i];

            UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(uiElement.featureID);
            uiElement.unlockData = unlockData;

            uiElement.button.onClick.AddListener(() =>
            {
                ToggleCanvas(uiElement, isLocked[index]);
            });
            uiElement.lockIcon.SetActive(isLocked[i]);

            uiElement.canvas = (UI_BottomElement)UIManager.instance.GetUIElement(ui_bases[i]);


            // if (uiElements[i].canvas.name == "UI_Castle(Clone)")
            // {
            uiElement.canvas.Initialize();
            // }

            uiElement.canvas.cloaseBtn.onClick.AddListener(() => CloseCanvas());

            uiElement.canvas.openUI += () => OpenCanvas(uiElement);

            uiElements[i] = uiElement;
        }
    }

    public void InitUnlock()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            int index = i;
            var uiElement = uiElements[i];

            if (uiElement.unlockData == null)
            {
                continue;
            }

            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(uiElement.unlockData.featureType, uiElement.unlockData.featureID, uiElement.unlockData.count,
            () =>
            {
                SetLockState(index, false);
            }));

            NotificationManager.instance.SetNotification(uiElement.redDotID.ToString(), false);
            uiElement.canvas.StartInit();
        }
    }

    public void SetLockState(int index, bool state)
    {
        if (index < 0 || index >= isLocked.Length) return;

        isLocked[index] = state;
        uiElements[index].lockIcon.SetActive(state);

        SaveDatas();
    }

    public void GameFailedCanvas()
    {
        ToggleCanvas(uiElements[1], isLocked[1]);
    }

    private void ToggleCanvas(UIElement element, bool locked)
    {
        if (locked)
        {
            Debug.Log("This element is locked.");
            if (element.unlockData.featureType == FeatureType.Level)
            {
                uI_Alert.AlertMessage($"캐릭터 레벨 <color=green>{element.unlockData.count}</color>을 달성해야 합니다.");
            }
            else if (element.unlockData.featureType == FeatureType.Stage)
            {
                uI_Alert.AlertMessage($"<color=green>{Difficulty.TransformStageNumber(element.unlockData.count)}</color>을 클리어 해야합니다.");
            }
            else if (element.unlockData.featureType == FeatureType.Quest)
            {
                uI_Alert.AlertMessage($"퀘스트 <color=green>{element.unlockData.count - 1}</color>을 클리어 해야합니다.");
            }
            return;
        }

        if (activeElement != null && activeElement.Value.canvas == element.canvas)
        {
            CloseCanvas();
        }
        else
        {
            if (activeElement != null)
            {
                CloseCanvas();
            }
            OpenCanvas(element);
        }
    }

    public void OpenDungeonCanvas()
    {
        ToggleCanvas(uiElements[3], isLocked[3]);
    }

    private void OpenCanvas(UIElement element)
    {
        activeElement = element;
        activeElementName = element.text.text;
        activeSprite = element.image.sprite;

        element.canvas.OpenUI();
        element.image.sprite = closeSprite;
        element.text.text = "닫기";

        //NotificationManager.instance.SetNotification(element.redDotID.ToString(), false);

    }

    private void CloseCanvas()
    {
        if (activeElement == null) return;

        activeElement.Value.canvas.CloseUI();
        activeElement.Value.image.sprite = activeSprite;
        activeElement.Value.text.text = activeElementName;

        // NotificationManager.instance.SetNotification(activeElement.Value.redDotID.ToString(), false);

        activeElement = null;
        activeElementName = null;
        activeSprite = null;
    }

    private void SaveDatas()
    {

        ES3.Save<bool[]>(Consts.IS_LOCKED, isLocked, ES3.settings);

        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        if (ES3.KeyExists(Consts.IS_LOCKED))
        {
            isLocked = ES3.Load<bool[]>(Consts.IS_LOCKED);
        }
    }
}