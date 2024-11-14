using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelSkillUIElement : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image coolTimeImage;
    [SerializeField] private Image lockIconImage;

    [SerializeField] private Button skillButton;

    [SerializeField] private Sprite addIconSprite;

    [SerializeField] private Button lockButton;

    private Coroutine preCoroutine;

    private float coolTime;

    private bool isCoolTimeProgress;

    private ColleagueData colleagueData;

    public event Func<ColleagueType, int, float> OnUseSkill;

    private bool isSkillExist;

    private WaitForSeconds skillUsingDuration;

    public event Action<MainPanelSkillUIElement> OnEnqueueUsingSkillElement;

    private Coroutine currentCoroutine;

    private Castle castle;

    private Sprite defaultBackgroundSprite;

    [SerializeField] private GameObject guide;

    private FeatureType featureType;
    private int unlockCount;

    public void Init()
    {
        skillButton.enabled = false;
        skillButton.onClick.AddListener(OnClickSkill);
        lockButton.onClick.AddListener(SendUnlockInfo);
        castle = FindAnyObjectByType<Castle>();
        castle.OnResetSkillCoolTime += ResetCooltime;
        defaultBackgroundSprite = backgroundImage.sprite;
        colleagueData.skillIndex = -1;
    }

    public int GetSkillIndex()
    {
        return colleagueData.skillIndex;
    }

    public void UpdateMainSprite(Sprite sprite)
    {
        skillButton.enabled = true;
        if (sprite == null)
        {
            mainImage.gameObject.SetActive(false);
            lockIconImage.sprite = addIconSprite;
            lockIconImage.color = Color.white;
            lockIconImage.gameObject.SetActive(true);
            if (lockButton.enabled)
            {
                lockButton.enabled = false;
                lockButton.onClick.RemoveListener(SendUnlockInfo);
            }
            //guide.gameObject.SetActive(true);
        }
        else
        {
            mainImage.sprite = sprite;
            mainImage.gameObject.SetActive(true);
            lockIconImage.gameObject.SetActive(false);
            OnEnqueueUsingSkillElement?.Invoke(this);
            //guide.gameObject.SetActive(false);
        }
    }

    public void UpdateBackgroundSprite(Sprite sprite)
    {
        backgroundImage.sprite = sprite;
    }

    public void ResetBackgroundSprite()
    {
        backgroundImage.sprite = defaultBackgroundSprite;
    }

    public void StopCalculateCoolTime()
    {
        isCoolTimeProgress = false;

        if (preCoroutine != null)
        {
            StopCoroutine(preCoroutine);
            preCoroutine = null;
            isSkillExist = false;
            coolTimeImage.fillAmount = 0f;
        }
    }

    public void UpdateCoolTimeState(float coolTime)
    {
        this.coolTime = coolTime;
    }

    private void OnClickSkill()
    {
        GetPossible();
    }

    public bool TryUsingSkill()
    {
        return GetPossible();
    }

    private bool GetPossible()
    {
        if (isCoolTimeProgress)
        {
            return isCoolTimeProgress;
        }

        if (colleagueData.skillIndex == -1)
        {
            UIManager.instance.GetUIElement<UI_Colleague>().openUI?.Invoke();
            return false;
        }
        
        float duration = OnUseSkill.Invoke(colleagueData.colleagueInfo.colleagueType, colleagueData.skillIndex);
        bool isPossible = !isCoolTimeProgress && isSkillExist && duration > 0f;

        if (isPossible)
        {
            isCoolTimeProgress = true;
            if (skillUsingDuration == null)
            {
                skillUsingDuration = CoroutineUtility.GetWaitForSeconds(duration);
            }

            currentCoroutine = StartCoroutine(UpdateCoolTime());
        }

        return isPossible;
    }

    private IEnumerator UpdateCoolTime()
    {
        yield return skillUsingDuration;

        coolTimeImage.gameObject.SetActive(true);

        float elapsedTime = Time.deltaTime;
        float ratio = 0f;

        while (ratio < 1f)
        {
            ratio = elapsedTime / coolTime;
            coolTimeImage.fillAmount = ratio;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        BeReadyToUsingSkill();
    }

    private void ResetCooltime()
    {
        if (currentCoroutine == null)
        {
            return;
        }

        StopCoroutine(currentCoroutine);
        BeReadyToUsingSkill();
    }

    private void BeReadyToUsingSkill()
    {
        coolTimeImage.gameObject.SetActive(false);

        isCoolTimeProgress = false;

        OnChangeAutoSkillState();
    }

    public void UpdateColleagueData(ColleagueData colleagueData)
    {
        this.colleagueData = colleagueData;
        isSkillExist = colleagueData.index != -1;
    }

    public void OnChangeAutoSkillState()
    {
        if (!isCoolTimeProgress)
        {
            OnEnqueueUsingSkillElement?.Invoke(this);
        }
    }

    public void SetUnlockDataValues(FeatureType featureType, int count)
    {
        this.featureType = featureType;
        unlockCount = count;
    }

    private void SendUnlockInfo()
    {
        UnlockManager.Instance.ToastLockMessage(featureType, unlockCount);
    }
}
