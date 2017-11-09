using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPrompt : MonoBehaviour
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
}
