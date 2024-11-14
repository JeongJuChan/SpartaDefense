using UnityEngine;

public class GuideStep : MonoBehaviour
{

    public void Show()
    {
        gameObject.SetActive(true);
        // 추가적인 로직이 필요하면 여기에 작성
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        // 추가적인 로직이 필요하면 여기에 작성
    }
}