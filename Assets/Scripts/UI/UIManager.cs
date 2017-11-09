using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.Constants;
using UI.Enums;

public class UIManager : Singleton<UIManager>
{
    // Constants
    private static readonly string Identifier = "UIManager";

    // Inspector Serialized Fields
    [SerializeField]
    private Canvas m_Canvas;                            //! The Game Canvas. We'll only use one and load prefabs for Screens and other elements Under it.

    [SerializeField]
    private Transform m_PromptsBar;

    [SerializeField]
    private GameObject m_PromptPrefab;

    [SerializeField]
    private List<UIScreenPrefabInfo> m_PrefabInfo;      //! List of Prefab Information: Includes ID to Prefab For Loading.

    // Private Member Variables
    private Animator m_ScreenAnimator;                  //! Reference Member Variable for the current screen animator. Use this to play animations and transitions.
    private Stack<ScreenId> m_ScreenStack;              //! Screen Stack by Id, so we can navigate the screen history.
    private UIBaseScreen m_CurrentScreen;               //! Reference to the current screen. We can use this to get data or input, and set up event listeners.

    // Locks
    private bool m_AnimationLock;                       //! Animation Lock, do not allow screens to animate while this is locked.
    private bool m_InputLock;                           //! Input Lock, block input while this is locked.
    private bool m_PrefabLoadingLock;                   //! Lock when loading a prefab, so we don't do anything else or load another prefab.

    public bool IsAnimationLocked { get { return m_AnimationLock; } set { m_AnimationLock = value; } }
    public bool IsInputLocked { get { return m_InputLock; } set { m_InputLock = value; } }
    public bool IsPrefabLoadingLocked { get { return m_PrefabLoadingLock; } set { m_PrefabLoadingLock = value; } }

    public override void Awake()
    {
        Debug.AssertFormat(ValidateManager() != false, "{0} : Failed to validate, please ensure that all required components are set and not null.", UIManager.Identifier);
        m_ScreenStack = new Stack<ScreenId>();
        base.Awake();
    }

    /// <summary>
    /// Validate that any serialized fields are properly set, and that
    /// the screens are valid.
    /// A Valid Manager should function properly.
    /// </summary>
    /// <returns></returns>
    protected override bool ValidateManager()
    {
        bool isValid = true;

        isValid = isValid && (m_Canvas != null);

        isValid = isValid && ValidateScreenList();

        isValid = isValid && base.ValidateManager();

        return isValid;
    }

    /// <summary>
    /// Validates that the screen list contains no duplicate screens.
    /// </summary>
    /// <returns>True if Valid.</returns>
    private bool ValidateScreenList()
    {
        bool isValid = false;

        isValid = (m_PrefabInfo != null) && (m_PrefabInfo.Count > 0);

        List<ScreenId> tempIds = new List<ScreenId>();

        if (isValid)
        {
            for (int i = 0; i < m_PrefabInfo.Count; i++)
            {
                if (!tempIds.Contains(m_PrefabInfo[i].ScreenId))
                {
                    tempIds.Add(m_PrefabInfo[i].ScreenId);
                }
                else
                {
                    isValid = false;
                    break;
                }
            }
        }

        return isValid;
    }

    /// <summary>
    /// Returns the prefab associated with a screen id.
    /// </summary>
    /// <param name="screenId"></param>
    /// <returns></returns>
    private GameObject GetPrefabFromScreenId(ScreenId screenId)
    {
        GameObject screenPrefab = null;

        if (screenId != ScreenId.None)
        {
            UIScreenPrefabInfo info = m_PrefabInfo.Find(p => p.ScreenId == screenId);
            screenPrefab = info.Prefab;
            return screenPrefab;
        }

        Debug.AssertFormat(true, "{0} : Couldn't find a prefab for screenid : {1}. Make sure it is serialzied in mPrefabInfo.", UIManager.Identifier, screenId.ToString());
        return screenPrefab;
    }

    /// <summary>
    /// Loads a screen based on it's screen id.
    /// </summary>
    /// <param name="screenId">Unique identifier for a screen.</param>
    private UIBaseScreen LoadScreen(ScreenId screenId)
    {
        GameObject screenPrefab = GetPrefabFromScreenId(screenId);

        if (screenPrefab != null)
        {
            // Instantiate the screen in the canvas.
            GameObject instantiatedPrefab = GameObject.Instantiate(screenPrefab, m_Canvas.transform);

            if (instantiatedPrefab != null)
            {
                UIBaseScreen screen = instantiatedPrefab.GetComponent<UIBaseScreen>();

                if (screen != null)
                {
                    // Call set defaults and assign the current screen.
                    screen.Initialize();

                    // make sure prompts bar is last in the hierarchy
                    m_PromptsBar.SetAsLastSibling();

                    return screen;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Unloads the current screen.
    /// Will likely do more stuff in the future.
    /// </summary>
    private void UnloadCurrentScreen()
    {
        if (m_CurrentScreen != null)
        {
            m_CurrentScreen.Shutdown();
            GameObject.Destroy(m_CurrentScreen.gameObject);
        }
    }

    // Go back! But only if we actually can.
    public void DoBackTransition()
    {
        if (m_ScreenStack.Count >= General.MINIMUM_NUM_SCREENS_FOR_BACK)
        {
            m_ScreenStack.Pop();
            TransitionToScreen(m_ScreenStack.Peek());
        }
    }

    /// <summary>
    /// Clear the current screen stack.. maybe when shutting down the UI?
    /// </summary>
    public void ClearScreenStack()
    {
        m_ScreenStack.Clear();
    }

    /// <summary>
    /// Transition to a screen by id.
    /// </summary>
    /// <param name="screenId"></param>
    public void TransitionToScreen(ScreenId screenId)
    {
        StartCoroutine(DoScreenTransition(screenId));
    }

    private void ClearPrompts()
    {
        for (int i = 0; i < m_PromptsBar.childCount; i++)
        {
            Destroy(m_PromptsBar.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Transition Work Enumerator.
    /// Carries out the screen loading process and locks the system until a screen has been loaded.
    /// </summary>
    /// <param name="screenId"></param>
    /// <returns></returns>
    private IEnumerator DoScreenTransition(ScreenId screenId)
    {
        // if this is a new screen... (it should always be.)
        if (screenId != ScreenId.None)
        {
            m_InputLock = true;
            bool canNavigateBackwards = false;

            if (m_CurrentScreen != null)
            {
                canNavigateBackwards = m_CurrentScreen.CanNavigateBack;
                yield return StartCoroutine(m_CurrentScreen.DoScreenAnimation(UIScreenAnimState.Outro));
                UnloadCurrentScreen();
            }

            m_PrefabLoadingLock = true;
            UIBaseScreen loadedScreen = LoadScreen(screenId);

            if (loadedScreen != null)
            {
                while (m_PrefabLoadingLock)
                {
                    yield return null;
                }

                // If the current screen doesn't support back navigation, remove it from the stack.
                if (!canNavigateBackwards && m_ScreenStack.Count > 0)
                {
                    m_ScreenStack.Pop();
                }

                m_CurrentScreen = loadedScreen;

                // Back transitions can't add the screen twice.
                if (m_ScreenStack.Count == 0 || (m_ScreenStack.Count > 0 && screenId != m_ScreenStack.Peek()))
                {
                    // Push the new screen onto the stack.
                    m_ScreenStack.Push(screenId);
                }

                yield return StartCoroutine(m_CurrentScreen.DoScreenAnimation(UIScreenAnimState.Intro));

                ClearPrompts();
                m_CurrentScreen.SetPrompts(m_PromptsBar, m_PromptPrefab);
            }
            m_InputLock = false;
        }
    }
}