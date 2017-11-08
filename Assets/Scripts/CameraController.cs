using UnityEngine;
using Rewired;

public class CameraController : MonoBehaviour
{
    public enum eCameraMode { Free, Locked, Follow, Static };

	[SerializeField] private Transform m_Target;
    [SerializeField] private Transform m_LockObject;
    [SerializeField] private Transform m_Transform;

    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Pitch = 2f;
    [SerializeField] private float m_Zoom = 5f;
    [SerializeField] private float m_YawSpeed = 100f;
    [SerializeField] private float m_LockedViewDistance = 5f;
    [SerializeField] private float m_MinCameraY = 0.3f;
    [SerializeField] private float m_CurrentYaw = 0f;

    private eCameraMode m_CameraMode = eCameraMode.Free;

    private Transform m_PlayerTransform;
    private float m_LerpSpeed = 1f;

    private void Awake()
    {
        InputManager.Instance.AddInputEventDelegate(OnInputUpdate, Rewired.UpdateLoopType.Update);
    }

    private void OnDestroy()
    {
        InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);
    }

    private void OnInputUpdate(InputActionEventData data)
    {
        
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

        if (m_CameraMode == eCameraMode.Free || m_CameraMode == eCameraMode.Follow)
        {
            if (m_CameraMode == eCameraMode.Free)
            {
                m_CurrentYaw += horizontal * m_YawSpeed * Time.deltaTime;
            }

            m_Offset.y += vertical * -1f * Time.deltaTime;
            m_Offset.y = Mathf.Clamp(m_Offset.y, -3f, -0.75f);
        }
        else
        {
            m_CurrentYaw = 0f;
        }
    }

    public void ChangeMode(eCameraMode mode)
    {
        m_CameraMode = mode;
    }

    private void LateUpdate()
    {
        if (m_Target != null)
        {
            if (m_CameraMode == eCameraMode.Free || m_CameraMode == eCameraMode.Static)
            {
                Vector3 newPosition = m_Target.position - m_Offset * m_Zoom;
                m_Transform.position = Vector3.Lerp(m_Transform.position, newPosition, m_LerpSpeed);
                m_Transform.LookAt(m_Target.position + Vector3.up * m_Pitch);
                m_Transform.RotateAround(m_Target.position, Vector3.up, m_CurrentYaw);
            }
            else if (m_CameraMode == eCameraMode.Locked)
            {

                Vector3 lockToTarget = (m_LockObject.position - m_Target.position).normalized;

                // don't adjust the camera's height, just rotation
                lockToTarget.y = 0f;

                //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(lockToTarget), m_LockRotationSpeed * Time.deltaTime);
                m_Transform.rotation = Quaternion.LookRotation(lockToTarget);
                m_Transform.eulerAngles = new Vector3(0, m_Transform.eulerAngles.y, 0);
                //m_Transform.position = Vector3.Slerp(m_Transform.position, (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance, Time.deltaTime);

                Vector3 cameraPos = (m_Target.position + Vector3.up) - lockToTarget * m_LockedViewDistance;
                if (cameraPos.y < m_MinCameraY)
                {
                    cameraPos.y = m_MinCameraY;
                }
                m_Transform.position = cameraPos;
            }
            else if (m_CameraMode == eCameraMode.Follow)
            {
                Vector3 newPosition = m_Target.position - m_Target.rotation * m_Offset * m_Zoom;
                m_Transform.position = Vector3.Lerp(m_Transform.position, newPosition, m_LerpSpeed);
                m_Transform.LookAt(m_Target.position + Vector3.up * m_Pitch);
            }
        }
    }
}
