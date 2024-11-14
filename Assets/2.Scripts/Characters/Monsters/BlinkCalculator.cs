using System.Collections;
using UnityEngine;

public class BlinkCalculator<T> where T: MonoBehaviour
{
    private T t;
    private SpriteRenderer spriteRenderer;
    private WaitForSeconds blinkWaitForSeconds;

    private Coroutine preCoroutine;

    private Color offsetColor;

    public BlinkCalculator(T t, SpriteRenderer spriteRenderer, float blinkDuration)
    {
        
        this.t = t;
        this.spriteRenderer = spriteRenderer;

        offsetColor = spriteRenderer.color;

        blinkWaitForSeconds = CoroutineUtility.GetWaitForSeconds(blinkDuration);

        if (t.TryGetComponent(out IDamagable damagable))
        {
            damagable.OnDamaged += Blink;
        }
        if (t.TryGetComponent(out IAlive alive))
        {
            alive.OnAlive += ResetBlink;
        }
    }

    private void Blink()
    {
        ResetBlink();
        if (t.gameObject.activeInHierarchy)
        {
            preCoroutine = t.StartCoroutine(CoBlink());
        }
    }

    private IEnumerator CoBlink()
    {
        spriteRenderer.color = Color.red;

        yield return blinkWaitForSeconds;
        ResetBlink();
    }

    private void ResetBlink()
    {
        if (preCoroutine != null)
        {
            t.StopCoroutine(preCoroutine);
        }

        spriteRenderer.color = offsetColor;
    }
}