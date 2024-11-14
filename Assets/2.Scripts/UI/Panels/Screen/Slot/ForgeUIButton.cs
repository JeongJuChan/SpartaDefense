using Keiwando.BigInteger;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeUIButton : UI_Base, UIInitNeeded
{
    [field: SerializeField] public Button forgeButton { get; private set; }
    [SerializeField] private TextMeshProUGUI forgeCountText;
    [SerializeField] private StageDefeatPanel stageDefeatPanel;
    [SerializeField] private AutoForgeUIPanel autoForgeUIPanel;
    [SerializeField] private GuideController guide;

    [SerializeField] private Button disableButton;

    private Animator animator;

    private int IS_TRIGGER_HASH = AnimatorParameters.IS_TRIGGER_HASH;

    public event Action<float> OnUpdateTriggerDuration;
    public event Action OnForgeSlot;
    public event Action<bool> UpdateAuto;

    [SerializeField] private float triggerDuration = 0.25f;

    private const string FORGE_TAG = "Forge";

    public event Func<bool> OnGetIsSlotEmpty;

    public event Action OnActiveSlotUIPopup;

    public event Action OnDeActivateAutoForgeDeactiveButton;
    public event Action OnActivateAutoForgeDeActiveButton;

    private Coroutine shakeCoroutine;

    private float lastTriggerTime;

    private int shakeCount = 0;
    private bool isShakingPossible = true;

    private bool isTriggerDurationSet = false;

    public void Init()
    {
        guide.Initialize();
        forgeButton.onClick.AddListener(TriggerForgeAnimation);
        stageDefeatPanel.OnForgeSlot += InvokeForgeButton;
        animator = GetComponent<Animator>();
        DeActiveDisableButton();
        CurrencyManager.instance.GetCurrency(CurrencyType.ForgeTicket).OnCurrencyChange += UpdateForgeUIText;
        autoForgeUIPanel.OnAutoForge += ForgeAuto;

        disableButton.onClick.AddListener(DeActiveDisableButton);
        disableButton.onClick.AddListener(autoForgeUIPanel.StopAuto);

        QuestManager.instance.AddQuestTypeAction(QuestType.ForgeSummonCount, 
            () => QuestManager.instance.UpdateCount(QuestType.ForgeSummonCount, 0));

        UpdateForgeUIText(CurrencyManager.instance.GetCurrencyValue(CurrencyType.ForgeTicket));
    }

    private void Update()
    {
        if (Time.time - lastTriggerTime >= 3f && shakeCoroutine == null && isShakingPossible)
        {
            isShakingPossible = false;
            shakeCoroutine = StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            UIAnimations.Instance.ShakeObject(forgeButton.gameObject);

            shakeCount++;

            if (shakeCount >= 3)
            {
                shakeCount = 0;
                if (shakeCoroutine != null)
                {
                    StopCoroutine(shakeCoroutine);
                    shakeCoroutine = null;
                    lastTriggerTime = Time.time;
                }
            }
        }
    }

    private void TriggerForgeAnimation()
    {
        TrySetTriggerDuration();

        if (!ForgeManager.instance.GetIsNewSlotSold())
        {
            OnActiveSlotUIPopup?.Invoke();
            return;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        DialogManager.instance.HideDialog();

        lastTriggerTime = Time.time;

        if (!OnGetIsSlotEmpty.Invoke())
        {
            OnActiveSlotUIPopup?.Invoke();
            return;
        }

        BigInteger forgeTicket = CurrencyManager.instance.GetCurrencyValue(CurrencyType.ForgeTicket);
        if (forgeTicket <= 0)
        {
            forgeButton.enabled = true;
            return;
        }

        forgeButton.enabled = false;
        OnActivateAutoForgeDeActiveButton?.Invoke();

        UpdateAuto?.Invoke(false);

        OnForgeSlot?.Invoke();


        if (UIManager.instance.isPopupOpened)
        {
            OnUpdateTriggerDuration?.Invoke(triggerDuration);
        }
        else
        {
            animator.SetBool(IS_TRIGGER_HASH, true);
            QuestManager.instance.UpdateCount(QuestType.ForgeSummonCount, 1);
            //StartCoroutine(CoTriggerForgeEvent());
        }
    }

    private void TrySetTriggerDuration()
    {
        if (!isTriggerDurationSet)
        {
            OnUpdateTriggerDuration?.Invoke(triggerDuration);
            isTriggerDurationSet = true;
        }
    }

    private void ForgeAuto(Rank rank)
    {
        TrySetTriggerDuration();

        if (!OnGetIsSlotEmpty.Invoke())
        {
            return;
        }

        BigInteger forgeTicket = CurrencyManager.instance.GetCurrencyValue(CurrencyType.ForgeTicket);
        if (forgeTicket <= 0)
        {
            forgeButton.enabled = true;
            DeActiveDisableButton();
            return;
        }

        UpdateAuto?.Invoke(true);
        OnForgeSlot?.Invoke();

        OnActivateAutoForgeDeActiveButton?.Invoke();
        ActiveDisableButton();

        if (UIManager.instance.isPopupOpened)
        {
            OnUpdateTriggerDuration?.Invoke(triggerDuration);
        }
        else
        {
            animator.SetBool(IS_TRIGGER_HASH, true);
            QuestManager.instance.UpdateCount(QuestType.ForgeSummonCount, 1);
            //StartCoroutine(CoTriggerForgeEvent());
        }
    }

    private IEnumerator CoTriggerForgeEvent()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!animatorStateInfo.IsTag(FORGE_TAG))
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        triggerDuration = animatorStateInfo.length;
        OnUpdateTriggerDuration?.Invoke(triggerDuration);

        QuestManager.instance.UpdateCount(QuestType.ForgeSummonCount, 1);
    }

    public void EndForge(bool isActiveForgeButton)
    {
        animator.SetBool(IS_TRIGGER_HASH, false);
        forgeButton.enabled = isActiveForgeButton;
        isShakingPossible = isActiveForgeButton;
    }

    public void SetOffForgeButton()
    {
        forgeButton.enabled = false;
    }

    public void SetOnForgeButton()
    {
        forgeButton.enabled = true;
    }

    private void ActiveDisableButton()
    {
        disableButton.gameObject.SetActive(true);
    }

    public void DeActiveDisableButton()
    {
        disableButton.gameObject.SetActive(false);
        OnDeActivateAutoForgeDeactiveButton?.Invoke();
    }

    private void InvokeForgeButton()
    {
        forgeButton.onClick.Invoke();
    }

    private void UpdateForgeUIText(BigInteger forgeTicketCount)
    {
        forgeCountText.text = forgeTicketCount.ToString();
    }
}
