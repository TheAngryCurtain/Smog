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
    [SerializeField] private Image m_ColorSwatch;
    [SerializeField] private float m_CursorSpeed = 10f;
    [SerializeField] private Color[] m_PaintColors;

    //private UIMenu m_ActiveMenu;
    private Color m_CurrentColor;
    private int m_ColorIndex = 0;

    public override void Initialize()
    {
        base.Initialize();

        m_Cursor.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        SetColor(m_PaintColors[m_ColorIndex]);
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

                // JUST TESTING
            case RewiredConsts.Action.Drift:
                if (data.GetButtonDown())
                {
                    m_ColorIndex = (m_ColorIndex + 1) % m_PaintColors.Length;
                    SetColor(m_PaintColors[m_ColorIndex]);
                }
                break;

            case RewiredConsts.Action.Paint:
                if (data.GetButtonShortPress())
                {
                    VSEventManager.Instance.TriggerEvent(new GameEvents.PaintRequestAtPositionEvent(m_Cursor.position, m_CurrentColor));
                }
                break;
        }

        cursorPos.x += horizontal * m_CursorSpeed * Time.deltaTime;
        cursorPos.y += vertical * m_CursorSpeed * Time.deltaTime;

        m_Cursor.position = cursorPos;
    }

    private void SetColor(Color c)
    {
        m_CurrentColor = c;
        m_ColorSwatch.color = m_CurrentColor;
    }

    public override void Shutdown()
    {
        base.Shutdown();
    }
}
