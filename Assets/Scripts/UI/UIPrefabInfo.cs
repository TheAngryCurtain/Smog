using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Enums;

namespace UI
{
    [System.Serializable]
    public class UIScreenPrefabInfo
    {
        [SerializeField]
        private ScreenId m_ScreenId;
        public ScreenId ScreenId { get { return m_ScreenId; } }

        [SerializeField]
        private GameObject m_Prefab;
        public GameObject Prefab { get { return m_Prefab; } }
    }
}