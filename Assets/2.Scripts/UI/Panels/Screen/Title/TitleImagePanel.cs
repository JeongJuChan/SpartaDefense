using UnityEngine;
using UnityEngine.UI;

public class TitleImagePanel : MonoBehaviour
{
    [SerializeField] private Button titleImageButton;
    [SerializeField] private Image startLogoImage;
    [SerializeField] private float blinkSpeed = 10f;
    [SerializeField] private LoadingUIPanel loadingUIPanel;

    private float elapsedTime;
    private Color transparentColor;

    private bool isLoaded = false;

    private bool isFirstTimeInstalled = true;
    [SerializeField] private Prologue prologue;

    private void Awake()
    {
        ES3.CacheFile();
        ES3.settings = new ES3Settings(ES3.Location.Cache);
        SceneLoad();
    }

    private void SceneLoad()
    {
        PushNotificationManager.instance.StartInit();
        SceneManagerEx.instance.PrepareSceneLoaded(SceneType.MainScene, loadingUIPanel.UpdateLoadingBar, Init);
    }

    private void ShowPrologue()
    {
        prologue.OnPrologueEnded += LoadMainScene;
        prologue.Init();
    }

    private void Init()
    {
        LoadDatas();

        if (isFirstTimeInstalled)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"first_startpoint_0");
            //titleImageButton.onClick.AddListener(ShowPrologue);
        }

        LoadMainSceneEx();
    }

    private void LoadMainSceneEx()
    {
        titleImageButton.onClick.AddListener(LoadMainScene);
        transparentColor = Color.white;
        transparentColor.a = 0f;
        loadingUIPanel.gameObject.SetActive(false);
        startLogoImage.gameObject.SetActive(true);
        isLoaded = true;
    }

    private void Update()
    {
        if (isLoaded)
        {
            BlinkStartLogo();
        }
    }

    private void BlinkStartLogo()
    {
        startLogoImage.color = Color.Lerp(Color.white, transparentColor, Mathf.Abs(Mathf.Cos(elapsedTime + Mathf.PI / 2)));
        elapsedTime += Time.deltaTime * blinkSpeed;
    }

    private void LoadMainScene()
    {
        SaveDatas();

        SceneManagerEx.instance.LoadSceneLoaded();
    }

    private void SaveDatas()
    {
        ES3.Save<bool>(Consts.IS_FIRST_TIME_INSTALLED, false, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        isFirstTimeInstalled = ES3.Load<bool>(Consts.IS_FIRST_TIME_INSTALLED, true, ES3.settings);
    }
}
