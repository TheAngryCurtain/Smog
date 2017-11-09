using UnityEngine;
using Rewired;

public class CameraController : MonoBehaviour
{
    #region Camera State
    public class CameraPreset
    {
        public Vector3 m_Offset;
        public float m_YawSpeed;
        public float m_PitchSpeed;
        public float m_Zoom;
        public float m_Pitch;
        public float m_LerpSpeed;
        public float m_LockedViewDist;
        public float m_MinY;

        public CameraPreset(Vector3 offset, float yawSpeed, float pitchSpeed, float zoom, float pitch, float lerpSpeed, float lockedDist, float minY)
        {
            m_Offset = offset;
            m_YawSpeed = yawSpeed;
            m_PitchSpeed = pitchSpeed;
            m_Zoom = zoom;
            m_Pitch = pitch;
            m_LerpSpeed = lerpSpeed;

            m_LockedViewDist = lockedDist;
            m_MinY = minY;
        }
    }

    public class CameraState
    {
        protected CameraPreset m_Preset;
        protected Transform m_CameraTransform;
        protected Transform m_Target;
        protected float m_CurrentYaw;

        public CameraState(CameraPreset preset)
        {
            m_Preset = preset;
        }

        public virtual void Enter(Transform cam, Transform target)
        {
            m_CameraTransform = cam;
            m_Target = target;
        }

        public virtual void HandleInput(InputActionEventData data) { }
        public virtual void UpdateState() { }
        public virtual void Exit() { }
    }
    #endregion

    #region Free Camera
    public class FreeCamera : CameraState
    {
        public FreeCamera(CameraPreset preset) : base(preset) { }

        public override void Enter(Transform cam, Transform target)
        {
            base.Enter(cam, target);
        }

        public override void HandleInput(InputActionEventData data)
        {
            base.HandleInput(data);

            float horizontal = 0f;
            float vertical = 0f;

            switch (data.actionId)
            {
                case RewiredConsts.Action.Camera_Horizontal:
                    horizontal = data.GetAxis();
                    break;

                case RewiredConsts.Action.Camera_Vertical:
                    vertical = data.GetAxis();
                    break;
            }

           
            m_CurrentYaw += horizontal * m_Preset.m_YawSpeed * Time.deltaTime;

            m_Preset.m_Offset.y += vertical * -m_Preset.m_PitchSpeed * Time.deltaTime;
            m_Preset.m_Offset.y = Mathf.Clamp(m_Preset.m_Offset.y, -3f, -0.75f);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (m_Target != null)
            {
                Vector3 newPosition = m_Target.position - m_Preset.m_Offset * m_Preset.m_Zoom;
                m_CameraTransform.position = Vector3.Lerp(m_CameraTransform.position, newPosition, m_Preset.m_LerpSpeed);

                m_CameraTransform.LookAt(m_Target.position + Vector3.up * m_Preset.m_Pitch);
                m_CameraTransform.RotateAround(m_Target.position, Vector3.up, m_CurrentYaw);
            }
        }
    }
    #endregion

    #region Follow Camera
    public class FollowCamera : CameraState
    {
        public FollowCamera(CameraPreset preset) : base(preset) { }

        public override void Enter(Transform cam, Transform target)
        {
            base.Enter(cam, target);
        }

        public override void HandleInput(InputActionEventData data)
        {
            base.HandleInput(data);

            float vertical = 0f;

            switch (data.actionId)
            {
                case RewiredConsts.Action.Camera_Vertical:
                    vertical = data.GetAxis();
                    break;
            }


            m_Preset.m_Offset.y += vertical * -m_Preset.m_PitchSpeed * Time.deltaTime;
            m_Preset.m_Offset.y = Mathf.Clamp(m_Preset.m_Offset.y, -3f, -0.75f);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Vector3 newPosition = m_Target.position - m_Target.rotation * m_Preset.m_Offset * m_Preset.m_Zoom;
            m_CameraTransform.position = Vector3.Lerp(m_CameraTransform.position, newPosition, m_Preset.m_LerpSpeed);

            m_CameraTransform.LookAt(m_Target.position + Vector3.up * m_Preset.m_Pitch);
        }
    }
    #endregion

    #region Lockon Camera
    public class LockOnCamera : CameraState
    {
        private Transform m_LockedTarget;

        public LockOnCamera(CameraPreset preset, Transform locked) : base(preset)
        {
            m_LockedTarget = locked;
        }

        public override void Enter(Transform cam, Transform target)
        {
            base.Enter(cam, target);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (m_LockedTarget != null)
            {
                Vector3 lockToTarget = (m_LockedTarget.position - m_Target.position).normalized;

                // don't adjust the camera's height, just rotation
                lockToTarget.y = 0f;

                //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(lockToTarget), m_LockRotationSpeed * Time.deltaTime);
                m_CameraTransform.rotation = Quaternion.LookRotation(lockToTarget);
                m_CameraTransform.eulerAngles = new Vector3(0, m_CameraTransform.eulerAngles.y, 0);
                //m_Transform.position = Vector3.Slerp(m_Transform.position, (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance, Time.deltaTime);

                Vector3 cameraPos = (m_Target.position + Vector3.up) - lockToTarget * m_Preset.m_LockedViewDist;
                if (cameraPos.y < m_Preset.m_MinY)
                {
                    cameraPos.y = m_Preset.m_MinY;
                }

                m_CameraTransform.position = cameraPos;
            }
        }
    }
    #endregion

    #region Customization Camera
    public class CustomizationCamera : CameraState
    {
        private float m_CustomYaw;
        private float m_CustomPitch;
        private float m_CustomZoom = 3f;

        private float m_CustomYawSpeed = 75f;
        private float m_CustomPitchSpeed = 75f;
        private float m_ZoomSpeed = 1f;

        public CustomizationCamera(CameraPreset preset) : base(preset) { }

        public override void Enter(Transform cam, Transform target)
        {
            base.Enter(cam, target);
        }

        public override void HandleInput(InputActionEventData data)
        {
            base.HandleInput(data);

            float horizontal = 0f;
            float vertical = 0f;
            float zoom = 0f;

            switch (data.actionId)
            {
                case RewiredConsts.Action.Camera_Horizontal:
                    horizontal = data.GetAxis();
                    break;

                case RewiredConsts.Action.Camera_Vertical:
                    vertical = data.GetAxis();
                    break;

                case RewiredConsts.Action.Accelerate:
                    zoom = -data.GetAxis();
                    break;

                case RewiredConsts.Action.Brake:
                    zoom = data.GetAxis();
                    break;
            }

            m_CustomYaw += horizontal * m_CustomYawSpeed * Time.deltaTime;

            m_CustomPitch += vertical * m_CustomPitchSpeed * Time.deltaTime;
            m_CustomPitch = Mathf.Clamp(m_CustomPitch, -5f, 80f);

            m_CustomZoom += zoom * m_ZoomSpeed * Time.deltaTime;
            m_CustomZoom = Mathf.Clamp(m_CustomZoom, 2f, 4f);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (m_Target != null)
            {
                Vector3 customPosition = m_Target.position + m_Target.forward * m_CustomZoom; 
                m_CameraTransform.position = Vector3.Lerp(m_CameraTransform.position, customPosition, m_Preset.m_LerpSpeed);

                m_CameraTransform.LookAt(m_Target.position);
                m_CameraTransform.RotateAround(m_Target.position, Vector3.up, -m_CustomYaw);
                m_CameraTransform.RotateAround(m_Target.position, m_CameraTransform.right, m_CustomPitch);
            }
        }
    }
    #endregion

    [SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_LockObject;
    [SerializeField] private Transform m_Transform;

    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Pitch = 2f;
    [SerializeField] private float m_Zoom = 5f;
    [SerializeField] private float m_YawSpeed = 100f;
    [SerializeField] private float m_PitchSpeed = 100f;
    [SerializeField] private float m_LockedViewDistance = 5f;
    [SerializeField] private float m_MinCameraY = 0.3f;
    [SerializeField] private float m_CurrentYaw = 0f;

    private float m_LerpSpeed = 1f;
    private CameraState m_CurrentState;

    private static CameraState m_FreeState;
    private static CameraState m_FollowState;
    private static CameraState m_LockedOnState;
    private static CameraState m_CustomizationState;

    private void Awake()
    {
        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, Rewired.UpdateLoopType.Update);

        CameraPreset preset = new CameraPreset(m_Offset, m_YawSpeed, m_PitchSpeed, m_Zoom, m_Pitch, m_LerpSpeed, m_LockedViewDistance, m_MinCameraY);
        m_FreeState = new FreeCamera(preset);
        m_FollowState = new FollowCamera(preset);
        m_LockedOnState = new LockOnCamera(preset, m_LockObject);
        m_CustomizationState = new CustomizationCamera(preset);
    }

    private void Start()
    {
        VSEventManager.Instance.AddListener<GameEvents.GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);
        VSEventManager.Instance.RemoveListener<GameEvents.GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameEvents.GameStateChangedEvent e)
    {
        if (e.State == eGameState.Customization)
        {
            ChangeState(m_CustomizationState);
        }
        else
        {
            ChangeState(m_FreeState);
        }
    }

    private void ChangeState(CameraState state)
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.Exit();
        }

        m_CurrentState = state;
        m_CurrentState.Enter(m_Transform, m_Target);
    }

    private void OnInputUpdate(InputActionEventData data)
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.HandleInput(data);
        }
    }

    private void LateUpdate()
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.UpdateState();
        }
    }
}
