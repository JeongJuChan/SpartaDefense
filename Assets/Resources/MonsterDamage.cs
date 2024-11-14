using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
    [SerializeField] private DamageImage prefab;

    private Queue<DamageImage> imagePool = new Queue<DamageImage>();
    private HashSet<DamageImage> imageOnField = new HashSet<DamageImage>();

    public void Init()
    {
        FindAnyObjectByType<BattleManager>().OnSpawnDamageUI += ShowDamage;
    }

    private void ShowDamage(BigInteger damage, DamageType damageType, int direction, Vector2 pos)
    {
        DamageImage image;

        if (imagePool.Count > 0)
        {
            image = imagePool.Dequeue();
        }
        else
        {
            image = Instantiate(prefab, transform);
            image.OnAnimationEnd += ReturnToPool;
        }

        image.gameObject.SetActive(true);
        image.transform.position = pos;
        image.ShowDamage(damage.ToString(), (int)damageType, direction);
    }

    private void ReturnToPool(DamageImage image)
    {
        image.gameObject.SetActive(false);
        imageOnField.Remove(image);
        imagePool.Enqueue(image);
    }
}
