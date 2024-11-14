using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonQuitPanel : UI_Base
{
    [SerializeField] private Button quitButton;

    [SerializeField] private TextMeshProUGUI announceText;
    [SerializeField] private Image progressImage;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject quitPanel;

    [SerializeField] private Sprite challengingSprite;
    [SerializeField] private Sprite clearSprite;

    public event Action<bool> OnDungeonQuit;
    public event Action OnStopShowingLoadingPanel;

    private const string ANNOUNCE_MESSAGE_CLEAR = "보스를 클리어 했습니다.\n잠시 후에 스테이지 화면으로 돌아갑니다.";
    private const string ANNOUNCE_MESSAGE_CHALLENGING = "보스가 성을 습격합니다.\n보스를 처치하고 보상을 수령하세요.";
    private const string PROGRESS_MESSAGE_CLEAR = "클리어";
    private const string PROGRESS_MESSAGE_CHALLENGING = "도전 중";

    public void Init()
    {
        quitButton.onClick.AddListener(OnClickQuitButton);
    }

    public void UpdateRewardSetting(bool isClear)
    {
        announceText.text = isClear ? ANNOUNCE_MESSAGE_CLEAR : ANNOUNCE_MESSAGE_CHALLENGING;
        progressImage.sprite = isClear ? clearSprite : challengingSprite;
        progressText.text = isClear ? PROGRESS_MESSAGE_CLEAR : PROGRESS_MESSAGE_CHALLENGING;
        quitPanel.SetActive(!isClear);
    }

    public void ActiveSelf(bool isActive)
    {
        if (!isActive)
        {
            UIManager.instance.GetUIElement<UI_Dungeon>().cloaseBtn.onClick.Invoke();
        }
        gameObject.SetActive(isActive);
    }

    private void OnClickQuitButton()
    {
        OnDungeonQuit?.Invoke(false);
        OnStopShowingLoadingPanel?.Invoke();
        UIManager.instance.GetUIElement<UI_Dungeon>().OpenUI();
        ActiveSelf(false);
    }

    
}
