using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationUIPopup : MonoBehaviour
{
    [SerializeField] private Image accelerationTicketRankImage;
    [SerializeField] private Image accelerationTicketImage;

    [SerializeField] private TextMeshProUGUI accelerationTicketCountText;

    [SerializeField] private Button subtractButton;
    [SerializeField] private Button addButton;
    [SerializeField] private Button subtractTenButton;
    [SerializeField] private Button addTenButton;

    [SerializeField] private TextMeshProUGUI accelerationPossibleText;

    [SerializeField] private Button useButton;

    [SerializeField] private Button otherDisableButton;

    [SerializeField] private StepGuide stepGuide;


    public event Action<BigInteger> OnUseTicket;

    private BigInteger currentAccelerationTicketCount;

    public event Func<BigInteger> OnGetUsableAccelerationTicketCount;

    public event Func<BigInteger, string> OnGetAccelTimeText;

    public void Init()
    {
        accelerationTicketRankImage.sprite = ResourceManager.instance.rank.GetRankBackgroundSprite(Rank.Epic);
        accelerationTicketImage.sprite = CurrencyManager.instance.GetCurrency(CurrencyType.AccelerationTicket).GetIcon();
        otherDisableButton.onClick.AddListener(() => ActivateSelf(false));
        otherDisableButton.onClick.AddListener(() => otherDisableButton.gameObject.SetActive(false));
        subtractButton.onClick.AddListener(() => UpdateTicketCount(-1));
        addButton.onClick.AddListener(() => UpdateTicketCount(1));
        subtractTenButton.onClick.AddListener(() => UpdateTicketCount(-10));
        addTenButton.onClick.AddListener(() => UpdateTicketCount(10));
        useButton.onClick.AddListener(OnClickUseButton);
        ActivateSelf(false);
        otherDisableButton.gameObject.SetActive(false);
    }

    public void ActivateSelf(bool isActive)
    {
        if (isActive)
        {
            otherDisableButton.gameObject.SetActive(true);
        }
        gameObject.SetActive(isActive);
    }

    public void SetTicketCount(BigInteger count)
    {
        currentAccelerationTicketCount = count;
        accelerationTicketCountText.text = count.ToString();
        UpdateAccelerationPossibleText();
        ActivateSelf(true);
    }

    private void UpdateAccelerationPossibleText()
    {
        accelerationPossibleText.text = $"가속 가능 시간: {OnGetAccelTimeText.Invoke(currentAccelerationTicketCount * Consts.MINUTE_PER_TICKET)}";
    }

    private void UpdateTicketCount(BigInteger count)
    {
        if (count < 0)
        {
            currentAccelerationTicketCount = currentAccelerationTicketCount + count < 0 ? 0 : currentAccelerationTicketCount + count;
        }
        else
        {
            BigInteger totalAccelerationCount = CurrencyManager.instance.GetCurrencyValue(CurrencyType.AccelerationTicket);
            currentAccelerationTicketCount = currentAccelerationTicketCount + count > totalAccelerationCount ?
                totalAccelerationCount : currentAccelerationTicketCount + count;
            BigInteger usableCount = OnGetUsableAccelerationTicketCount.Invoke();
            currentAccelerationTicketCount = currentAccelerationTicketCount > usableCount ?
                usableCount : currentAccelerationTicketCount;
        }

        accelerationTicketCountText.text = currentAccelerationTicketCount.ToString();
        UpdateAccelerationPossibleText();
    }

    private void OnClickUseButton()
    {
        if (currentAccelerationTicketCount == 0)
        {
            UIManager.instance.GetUIElement<UI_Alert>().AlertMessage("사용할 가속 티켓이 없습니다.");
            return;
        }

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.AccelerationTicket, -currentAccelerationTicketCount);

        OnUseTicket?.Invoke(currentAccelerationTicketCount * Consts.MINUTE_PER_TICKET);
        currentAccelerationTicketCount = 0;
        ActivateSelf(false);
        otherDisableButton.gameObject.SetActive(false);

        stepGuide.NextStep(4);
    }
}
