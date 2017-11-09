using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuItem : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private Text m_Label;

    public void SetIcon(Sprite sprite)
    {
        m_Icon.sprite = sprite;
    }

    public void SetLabel(string text)
    {
        m_Label.text = text;
    }

    public void Highlight(bool highlight)
    {
        if (highlight)
        {
            m_Label.color = Color.yellow;
            m_Icon.rectTransform.localScale = new Vector3(1.25f, 1.25f);
        }
        else
        {
            m_Label.color = Color.white;
            m_Icon.rectTransform.localScale = Vector3.one;
        }
    }
}
