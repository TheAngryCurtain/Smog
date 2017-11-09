using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;
using UI.Enums;
using UI.Constants;

namespace UI
{
    public class UIBaseScreen : MonoBehaviour
    {
        ///////////////////////////////////////////////////////////////////
        /// Inspector Serialized Fields
        ///////////////////////////////////////////////////////////////////

        [SerializeField]
        private ScreenId m_ScreenId = ScreenId.None;                    //! Screen ID for this Screen. Editable in Inspector, do not modify at run time.
        public ScreenId ScreenId { get { return m_ScreenId; } }

        [SerializeField]
        private Vector3 m_ScreenPos = Vector3.zero;                     //! Initial Screen Position within the Canvas

        [SerializeField]
        private Vector3 m_ScreenScale = Vector3.one;                   //! Initial Screen Scale within the Canvas

        [SerializeField]
        private Quaternion m_ScreenRotation = Quaternion.identity;      //! Initial Screen Rotation within the Canvas

        [SerializeField]
        private Animator m_Animator;                                    //! Animator for this screen, should control intro, outro, etc.

        [SerializeField]
        private bool m_CanBack;                                         //! Can this screen navigate backwards?
        public bool CanNavigateBack { get { return m_CanBack; } }

        [SerializeField]
        protected List<UIPromptInfo> m_PromptInfo;

        protected bool m_HasSubSectionFocus = false;                    //! Does this screen have a subsection focused? If so, you can't back out 

        ///////////////////////////////////////////////////////////////////
        /// MonoBehaviour Implementation
        ///////////////////////////////////////////////////////////////////

        public virtual void Awake()
        {
            Debug.AssertFormat(ValidateScreen() != false, "Screen with id {0} failed to validate.", m_ScreenId);
        }

        public virtual void Start()
        {
            // After Start, we're good.
            UIManager.Instance.IsPrefabLoadingLocked = false;
        }

        public virtual void Update()
        {
        }

        ///////////////////////////////////////////////////////////////////
        /// UIBaseScreen Implementation
        ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// Validate the screen. Ensure any child classes of UIBaseScreen include validation for their own parameters.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateScreen()
        {
            bool isValid = false;

            isValid = (ScreenId != ScreenId.None);

            //isValid = isValid && (m_Animator != null);

            return isValid;
        }

        /// <summary>
        /// Set prompts.
        /// TODO: Implement Prompts.
        /// </summary>
        public virtual void SetPrompts(Transform parent, GameObject prefab)
        {
            for (int i = 0; i < m_PromptInfo.Count; i++)
            {
                GameObject promptObj = (GameObject)Instantiate(prefab, parent);
                UIPrompt prompt = promptObj.GetComponent<UIPrompt>();
                if (prompt != null)
                {
                    prompt.SetIcon(m_PromptInfo[i].m_IconSprite);
                    prompt.SetLabel(m_PromptInfo[i].m_LabelText);
                }
                else
                {
                    Debug.LogErrorFormat("No prompt script attached to {0} on the {1} screen.", promptObj.name, name);
                }
            }
        }

        /// <summary>
        /// Set up any initial data or state information.
        /// </summary>
        public virtual void Initialize()
        {
            // Do this because sometimes parenting screens to nodes can cause them to break their desired scale/rot/pos.
            transform.localPosition = m_ScreenPos;
            transform.localScale = m_ScreenScale;
            transform.localRotation = m_ScreenRotation;

            // Hook up our input listening.
            InputManager.Instance.AddInputEventDelegate(OnInputUpdate, UpdateLoopType.Update);
        }

        /// <summary>
        /// Destory anything that keeps memory, remove any callbacks, whatever.
        /// </summary>
        public virtual void Shutdown()
        {
            Debug.Log("Shutting down screen: " + name);

            // Remove our input listening.
            InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);
        }

        /// <summary>
        /// Handles input from Rewired.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnInputUpdate(InputActionEventData data)
        {
            //switch (data.actionId)
            //{
            //    case RewiredConsts.Action.UI_Cancel:
            //        if (data.GetButtonDown())
            //        {
            //            if (m_CanBack && !m_HasSubSectionFocus)
            //            {
            //                UIManager.Instance.DoBackTransition();
            //            }
            //        }
            //        break;
            //}
        }

        /// <summary>
        /// What our screens do when animation events occur. Use this to lock input, stop animations, enable audio, whatever.
        /// </summary>
        /// <param name="animEvent">The Animation Event, From Mecanim</param>
        public virtual void OnUIScreenAnimEvent(UIScreenAnimEvent animEvent)
        {
            switch (animEvent)
            {
                case UIScreenAnimEvent.None:
                    break;
                case UIScreenAnimEvent.Start:
                    UIManager.Instance.IsAnimationLocked = true;
                    break;
                case UIScreenAnimEvent.End:
                    UIManager.Instance.IsAnimationLocked = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Screen Animation Routine.
        /// 
        /// </summary>
        /// <param name="state">The Animation State, From Game Code.</param>
        /// <returns></returns>
        public virtual IEnumerator DoScreenAnimation(UIScreenAnimState state)
        {
            // Wait for the animation to finish.
            while (UIManager.Instance.IsAnimationLocked)
            {
                yield return null;
            }

            UIManager.Instance.IsAnimationLocked = true;
            PlayScreenAnimation(state);

            while (UIManager.Instance.IsAnimationLocked)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Set Animation Parameters, and let Mecanim Take Over.
        /// </summary>
        /// <param name="state">The Animation State to Transition To.</param>
        public virtual void PlayScreenAnimation(UIScreenAnimState state)
        {
            if (m_Animator != null)
            {
                m_Animator.SetTrigger(GetAnimatorParamFromState(state));
            }
            else
            {
                UIManager.Instance.IsAnimationLocked = false;
            }
        }

        /// <summary>
        /// Provide the Mecanim string that is associated with the state enum, so that we can do animations in code without 
        /// using strings like barbarians.
        /// </summary>
        /// <param name="state">State to get Mecanim String For</param>
        /// <returns></returns>
        private string GetAnimatorParamFromState(UIScreenAnimState state)
        {
            string param = Animations.MECANIM_NONE;
            switch (state)
            {
                case UIScreenAnimState.None:
                    param = Animations.MECANIM_NONE;
                    break;
                case UIScreenAnimState.Intro:
                    param = Animations.MECANIM_INTRO;
                    break;
                case UIScreenAnimState.Outro:
                    param = Animations.MECANIM_OUTRO;
                    break;
                default:
                    Debug.AssertFormat(true, "{0} : The state {1} is not a valid state for an animation transition.", gameObject.name, state.ToString());
                    break;
            }

            return param;
        }
    }
}