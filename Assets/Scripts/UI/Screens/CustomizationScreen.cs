using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Rewired;

public class CustomizationScreen : UIBaseScreen
{
    //[SerializeField] private UIMenu m_Menu;
    //[SerializeField] private UIMenu m_PlayersSubMenu;
    //[SerializeField] private UIMenu m_DifficultySubMenu;
    //[SerializeField] private Text m_Description;
    [SerializeField] private RectTransform m_Cursor;
    [SerializeField] private float m_CursorSpeed = 10f;

    //private UIMenu m_ActiveMenu;

    public override void Initialize()
    {
        base.Initialize();

        m_Cursor.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        float horizontal = 0f;
        float vertical = 0f;

        Vector2 cursorPos = m_Cursor.position;

        switch (data.actionId)
        {
            case RewiredConsts.Action.Steer_Horizontal:
                horizontal = data.GetAxis();
                break;

            case RewiredConsts.Action.Steer_Vertical:
                vertical = data.GetAxis();
                break;

            case RewiredConsts.Action.Paint:
                if (data.GetButtonShortPress())
                {
                    VSEventManager.Instance.TriggerEvent(new GameEvents.PaintRequestAtPositionEvent(m_Cursor.position));
                }
                break;
        }

        cursorPos.x += horizontal * m_CursorSpeed * Time.deltaTime;
        cursorPos.y += vertical * m_CursorSpeed * Time.deltaTime;

        m_Cursor.position = cursorPos;
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }
}
