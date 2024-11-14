using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private BackgroundSpriteSO backgroundSpriteSO;

    public void Init()
    {
        Vector2 size = new Vector2(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);

        Debug.Log(size);

        float cameraPixelHeight = Camera.main.orthographicSize * Consts.DEFAULT_PIXEL_PER_UNIT;
        float pivotRatio = spriteRenderer.sprite.pivot.x / spriteRenderer.sprite.pivot.y * Consts.HALF;

        Vector2 resolution = new Vector2(Screen.width, Screen.height);
        float scale = 0f;

        if (size.x >= size.y)
        {
            if (resolution.x >= size.x)
            {
                scale = cameraPixelHeight / size.y * resolution.x / size.x * pivotRatio;
            }
            else
            {
                scale = cameraPixelHeight / size.y * pivotRatio;
            }
        }
        else
        {
            scale = resolution.x / size.x * pivotRatio;
        }

        transform.localScale = new Vector3(scale, scale, 1f);

        FindAnyObjectByType<StageController>().OnMainStageUpdated += UpdateBackgroundSprite;
        FindAnyObjectByType<PlaceEventSwitcher>().OnBackToStage += UpdateBackgroundSprite;
    }

    private void UpdateBackgroundSprite(int repeatedMainStageNum)
    {
        spriteRenderer.sprite = backgroundSpriteSO.GetBackgroundSprites(repeatedMainStageNum);
    }
}
