using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CustomizationManager : Singleton<CustomizationManager>
{
    private bool m_Customizing = false;
    private RaycastHit m_Hit;

    private void Start()
    {
        VSEventManager.Instance.AddListener<GameEvents.GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameEvents.GameStateChangedEvent e)
    {
        if (e.State == eGameState.Customization)
        {
            InitCustomization();
        }
        else
        {
            CloseCustomization();
        }
    }

    private void InitCustomization()
    {
        if (!m_Customizing)
        {
            m_Customizing = true;
            VSEventManager.Instance.AddListener<GameEvents.PaintRequestAtPositionEvent>(OnPaintRequested);
        }
    }

    private void OnPaintRequested(GameEvents.PaintRequestAtPositionEvent e)
    {
        Debug.LogFormat("POS: {0}", e.Position);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(e.Position), out m_Hit))
        {
            PaintableMesh pMesh = m_Hit.collider.GetComponent<PaintableMesh>();
            if (pMesh != null)
            {
                pMesh.ApplyPaint(m_Hit.point, 0.1f, 0.2f, Color.red);
            }
        }
    }

    private void CloseCustomization()
    {
        if (m_Customizing)
        {
            m_Customizing = false;
            VSEventManager.Instance.RemoveListener<GameEvents.PaintRequestAtPositionEvent>(OnPaintRequested);
        }
    }
}
