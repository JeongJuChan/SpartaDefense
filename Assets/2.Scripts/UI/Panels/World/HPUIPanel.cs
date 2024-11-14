using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Keiwando.BigInteger;

public abstract class HPUIPanel : MonoBehaviour
{
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected Image damageFill;

    protected BigInteger maxHP = new BigInteger(0);
    protected BigInteger preHP = new BigInteger(0);

    protected float width;
    protected RectTransform damageRect;

    [SerializeField] protected float damageFillDecresingDuration = 1f;
    protected Coroutine preCoDamageFillDecrement;

    protected IHasHpUI hasHPUI;

    public virtual void init(IHasHpUI hasHPUI)
    {
        this.hasHPUI = hasHPUI;
        damageRect = damageFill.rectTransform;
        width = damageRect.sizeDelta.x;

        hasHPUI.OnResetHPUI += ResetUI;
        hasHPUI.OnUpdateMaxHPUI += UpdateMaxHP;
        hasHPUI.OnUpdateCurrenHPUI += UpdateCurrentHPUI;
        hasHPUI.OnActiveHpUI += ActiveSelf;
    }

    public virtual void ResetUI()
    {
        hpSlider.value = 1;
        SwitchDamageFillActive(false);
        preHP = maxHP;
    }

    public virtual void UpdateMaxHP(BigInteger maxHP, BigInteger currentHp)
    {
        this.maxHP = maxHP;
        preHP = currentHp;
        UpdateCurrentHPUI(preHP);
    }

    public virtual void UpdateCurrentHPUI(BigInteger currentHp)
    {
        if (maxHP == 0)
        {
            return;
        }
        float ratio = currentHp.ToFloat() / maxHP.ToFloat();
        hpSlider.value = ratio;
        if (preHP > currentHp)
        {
            if (preCoDamageFillDecrement != null)
            {
                StopCoroutine(preCoDamageFillDecrement);
            }

            SwitchDamageFillActive(true);
            InitDamageFillSize(currentHp);
            SetDamageFillPos(ratio);
            preCoDamageFillDecrement = StartCoroutine(CoDamageFillDecrease());

        }

        preHP = currentHp;
    }

    private void SetDamageFillPos(float currentValue)
    {
        Vector2 position = damageRect.anchoredPosition;
        position.x = width * currentValue;
        damageRect.anchoredPosition = position;
    }

    private void InitDamageFillSize(BigInteger currentHp)
    {
        Vector2 size = damageRect.sizeDelta;
        size.x = width * ((preHP - currentHp).ToFloat() / maxHP.ToFloat());
        damageRect.sizeDelta = size;
    }

    private IEnumerator CoDamageFillDecrease()
    {
        float elapsedTime = Time.deltaTime;

        float ratio = elapsedTime / damageFillDecresingDuration;

        while (ratio < 1f)
        {
            SetDamageFillSize(1f - ratio);
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / damageFillDecresingDuration;
            yield return null;
        }

        SwitchDamageFillActive(false);
    }

    private void SetDamageFillSize(float ratio)
    {
        Vector2 size = damageRect.sizeDelta;
        size.x = Mathf.Lerp(0, size.x, ratio);
        damageRect.sizeDelta = size;
    }

    private void SwitchDamageFillActive(bool isActive)
    {
        damageFill.gameObject.SetActive(isActive);
    }

    private void ActiveSelf(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            if (preCoDamageFillDecrement != null)
            {
                StopCoroutine(preCoDamageFillDecrement);
            }

            gameObject.SetActive(false);
        }
    }
}
