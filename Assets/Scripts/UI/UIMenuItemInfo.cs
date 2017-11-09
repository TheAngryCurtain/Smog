using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIMenuItemInfo
{
    public Sprite m_IconSprite;
    public string m_LabelText;
    public string m_Description; // this should be moved from here to a new MenuItemDescriptionInfo created
    public bool m_Togglable = false; // this should be moved from here to a new SettingsMenuItemInfo created
    public Sprite m_ThumbnailSprite; // this should be moved from here to a new MenuItemDescriptionThumbInfo created
}
