using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour
{
    public System.Action<int> OnItemSelected;
    public System.Action<UIMenuItemInfo> OnItemHighlighted;

    [SerializeField] private UIMenuItemInfo[] m_MenuItems;

    [SerializeField] private Transform m_MenuContainer;
    [SerializeField] private GameObject m_MenuItemPrefab;

    private float m_ScrollDelay = 0.25f;
    private float m_CurrentTime = 0f;

    private UIMenuItem[] m_ListItems;
    private UIMenuItem m_ActiveItem;
    private int m_ActiveIndex = 0;

    public void PopulateMenu()
    {
        m_ListItems = new UIMenuItem[m_MenuItems.Length];
        for (int i = 0; i < m_MenuItems.Length; i++)
        {
            GameObject itemObj = (GameObject)Instantiate(m_MenuItemPrefab, m_MenuContainer);
            UIMenuItem item = itemObj.GetComponent<UIMenuItem>();
            if (item != null)
            {
                item.SetIcon(m_MenuItems[i].m_IconSprite);
                item.SetLabel(m_MenuItems[i].m_LabelText);

                m_ListItems[i] = item;
            }
        }

        SetActiveItem(m_ListItems[m_ActiveIndex]);
    }

    public void PreSetMenuDataForModes(GameModeData[] data)
    {
        m_MenuItems = new UIMenuItemInfo[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            m_MenuItems[i] = new UIMenuItemInfo();
            m_MenuItems[i].m_IconSprite = data[i].m_Icon;
            m_MenuItems[i].m_LabelText = data[i].m_Name;
            m_MenuItems[i].m_Description = data[i].m_Description;
        }
    }

    public void ClearMenu()
    {
        for (int i = 0; i < m_MenuContainer.childCount; i++)
        {
            m_ListItems[i] = null;
            Destroy(m_MenuContainer.GetChild(i).gameObject);
        }
    }

    private void SetActiveItem(UIMenuItem item)
    {
        if (m_ActiveItem != null && m_ActiveItem != item)
        {
            m_ActiveItem.Highlight(false);
        }

        m_ActiveItem = item;
        m_ActiveItem.Highlight(true);
        EventSystem.current.SetSelectedGameObject(m_ActiveItem.gameObject);

        if (OnItemHighlighted != null)
        {
            OnItemHighlighted(m_MenuItems[m_ActiveIndex]);
        }
    }

    public void RefocusMenu()
    {
        m_ActiveIndex = 0;
        SetActiveItem(m_ListItems[m_ActiveIndex]);
    }

    public void RemoveMenuFocus()
    {
        if (m_ActiveItem != null)
        {
            m_ActiveItem.Highlight(false);
            m_ActiveItem = null;
        }
    }

    public void HandleInput(Rewired.InputActionEventData data)
    {
        //switch (data.actionId)
        //{
        //    case RewiredConsts.Action.UI_Horizontal:
        //        float value = data.GetAxis();
        //        if (value != 0f && m_MenuItems[m_ActiveIndex].m_Togglable)
        //        {
        //            // will need to figure out how to cut out of this. will likely need to change the switch to an if/else
        //        }
        //        break;

        //    case RewiredConsts.Action.UI_Vertical:
        //        value = data.GetAxis();
        //        if (value != 0f && m_CurrentTime <= 0f)
        //        {
        //            if (value < 0f)
        //            {
        //                m_ActiveIndex = (m_ActiveIndex + 1) % m_MenuItems.Length;
        //            }
        //            else if (value > 0f)
        //            {
        //                if (m_ActiveIndex - 1 < 0)
        //                {
        //                    m_ActiveIndex = m_MenuItems.Length;
        //                }

        //                m_ActiveIndex -= 1;
        //            }

        //            SetActiveItem(m_ListItems[m_ActiveIndex]);
        //            m_CurrentTime = m_ScrollDelay;
        //        }
        //        m_CurrentTime -= Time.deltaTime;
        //        break;

        //    case RewiredConsts.Action.UI_Confirm:
        //        if (data.GetButtonDown())
        //        {
        //            OnItemSelected(m_ActiveIndex);
        //        }
        //        break;
        //}
    }
}
