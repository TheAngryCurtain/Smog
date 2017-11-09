using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameManager : Singleton<GameManager>
{
    private bool m_Paused = false;

    public override void Awake()
    {
        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, Rewired.UpdateLoopType.Update);
    }

    private void Start()
    {
        ChangeState(eGameState.InGame);
    }

    private void OnInputUpdate(InputActionEventData data)
    {
        switch (data.actionId)
        {
            case RewiredConsts.Action.Customize:
                if (data.GetButtonDown())
                {
                    if (m_Paused)
                    {
                        ChangeState(eGameState.InGame);
                    }
                    else
                    {
                        ChangeState(eGameState.Customization);
                    }

                    m_Paused = !m_Paused;
                }
                break;
        }
    }

    private void ChangeState(eGameState state)
    {
        switch (state)
        {
            case eGameState.InGame:
                UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.HUD);
                break;

            case eGameState.Customization:
                UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Customization);
                break;
        }

        VSEventManager.Instance.TriggerEvent(new GameEvents.GameStateChangedEvent(state));
    }
}
