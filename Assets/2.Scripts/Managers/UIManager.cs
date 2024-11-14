using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviorSingleton<UIManager>
{
    private Dictionary<string, UI_Base> uiElements = new Dictionary<string, UI_Base>();
    private Stack<UI_Base> uiOnScreen = new Stack<UI_Base>();
    private Dictionary<string, UI_Base> uiNotDestroyedOnLoad = new Dictionary<string, UI_Base>();
    public BottomBarController bottomBarController;

    public bool isPopupOpened { get; private set; }

    public void Init()
    {
        // FindButtomBar();

        foreach (var uiInitNeeded in GetComponentsInChildren<UIInitNeeded>(true))
        {
            if (uiInitNeeded != null)
            {
                uiInitNeeded.Init();

                if (uiInitNeeded is BottomBarController)
                {
                    bottomBarController = uiInitNeeded as BottomBarController;
                }
            }
        }
    }

    // private void FindButtomBar()
    // {
    //     var bottomBarController = FindObjectsByType<UI_BottomElement>(FindObjectsSortMode.None);

    //     foreach (var bottomBar in bottomBarController)
    //     {
    //         uiElements.Add(bottomBar.name, bottomBar);
    //     }
    // }


    public T GetUIElement<T>() where T : UI_Base
    {
        string name = typeof(T).Name;

        if (uiElements.TryGetValue(name, out UI_Base ui))
        {
            return ui as T;
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/{name}");

            if (prefab == null)
            {
                T t = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                t.OnOpenUI += OnOpenUI;
                t.OnCloseUI += OnCloseUI;
                uiElements[name] = t;
                return t;
            }

#if UNITY_EDITOR
            Debug.Assert(prefab != null, "UI prefab does not exit.");
#endif
             
            GameObject obj = Instantiate(prefab);

            obj.TryGetComponent<T>(out T uiElement);

            if (uiElement.notDestoryedOnLoad)
            {
                uiNotDestroyedOnLoad[name] = uiElement;
                DontDestroyOnLoad(obj);
            }

#if UNITY_EDITOR
            Debug.Assert(uiElement != null, "UI Componenet does not exit on the prefab.");
#endif

            obj.SetActive(false);
            uiElement.OnOpenUI += OnOpenUI;
            uiElement.OnCloseUI += OnCloseUI;
            uiElements[name] = uiElement;

            return uiElement;
        }
    }

    public UI_Base GetUIElement(UI_Base target)
    {
        string name = target.name;

        if (uiElements.TryGetValue(name, out UI_Base ui))
        {
            return ui;
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/{name}");
#if UNITY_EDITOR
            Debug.Assert(prefab != null, "UI prefab does not exit.");
#endif

            GameObject obj = Instantiate(prefab);

            obj.TryGetComponent(out UI_Base uiElement);

            if (uiElement.notDestoryedOnLoad)
            {
                uiNotDestroyedOnLoad[name] = uiElement;
                DontDestroyOnLoad(obj);
            }

#if UNITY_EDITOR
            Debug.Assert(uiElement != null, "UI Componenet does not exit on the prefab.");
#endif

            obj.SetActive(false);
            uiElement.OnOpenUI += OnOpenUI;
            uiElement.OnCloseUI += OnCloseUI;
            uiElements[name] = uiElement;

            return uiElement;
        }
    }

    public void ChangeIsPopupOpened(bool isActive)
    {
        isPopupOpened = isActive;
    }

    public void ClearCollections()
    {
        uiElements.Clear();

        foreach (KeyValuePair<string, UI_Base> kvp in uiNotDestroyedOnLoad)
        {
            uiElements[kvp.Key] = kvp.Value;
        }
    }

    private void OnOpenUI(UI_Base ui)
    {
        uiOnScreen.Push(ui);
    }

    private void OnCloseUI(UI_Base ui)
    {
        UI_Base topUi = null;
        Stack<UI_Base> tempStack = new Stack<UI_Base>();

        while (topUi != ui && uiOnScreen.Contains(ui))
        {
            uiOnScreen.TryPop(out topUi);
            tempStack.Push(topUi);
        }

        tempStack.TryPop(out topUi);

        while (tempStack.Count > 0)
        {
            uiOnScreen.Push(tempStack.Pop());
        }
    }
}
