using UnityEngine;
using Rewired;

public class VehicleController : MonoBehaviour
{
    [SerializeField] protected float _maxTorque;
    [SerializeField] protected float _maxBrake;
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _maxSteerAngle;
    [SerializeField] protected Axle[] _axles;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected Transform[] _seats;
    [SerializeField] protected Transform _centerOfMass;
    [SerializeField] protected ParticleSystem _exhaustParticle;

    [SerializeField] protected float _driftWheelFriction;

    private Transform _actionLocation;
    private Axle _currentAxle;

    private float _steerValue;
    private float _fowardThrottle;
    private float _reverseThrottle;
    private float _defaultWheelFriction;

    private Vector3 _activeWheelPos;
    private Quaternion _activeWheelRot;
    private float _currentSpeed;
    private float _directionSign;
    private float _deltaMotorForce;
    private float _currentMaxSteerAngle;
    private float _currentSteerAngle;

    public float CurrentSpeed { get { return _currentSpeed; } }
    public Transform DriverSeat { get { return _seats[0]; } }

    [System.Serializable]
    public class Axle
    {
        public WheelCollider LeftWheelCollider;
        public WheelCollider RightWheelCollider;

        public Transform LeftWheelTransform;
        public Transform RightWheelTransform;

        public bool Motor;
        public bool Steer;
    }

    private void OnGameStateChanged(GameEvents.GameStateChangedEvent e)
    {
        if (e.State == eGameState.InGame)
        {
            // grab control
            InputManager.Instance.AddInputEventDelegate(OnInputUpdate, Rewired.UpdateLoopType.Update);

            _exhaustParticle.Play();
        }
        else
        {
            // release control
            InputManager.Instance.RemoveInputEventDelegate(OnInputUpdate);

            _exhaustParticle.Stop();
        }
    }

    protected virtual void OnInputUpdate(InputActionEventData data)
    {
        switch (data.actionId)
        {
            case RewiredConsts.Action.Steer_Horizontal:
                _steerValue = data.GetAxis();
                break;

            case RewiredConsts.Action.Accelerate:
                _fowardThrottle = data.GetAxis();
                break;

            case RewiredConsts.Action.Brake:
                _reverseThrottle = data.GetAxis();
                break;

            case RewiredConsts.Action.Drift:
                if (data.GetButtonDown())
                {
                    ChangeSidewaysWheelFriction(_driftWheelFriction);
                }
                else if (data.GetButtonUp())
                {
                    ChangeSidewaysWheelFriction(_defaultWheelFriction);
                }
                break;
        }
    }

    protected virtual void Start()
    {
        VSEventManager.Instance.AddListener<GameEvents.GameStateChangedEvent>(OnGameStateChanged);

        _rigidbody.centerOfMass = _centerOfMass.localPosition;
        _defaultWheelFriction = _axles[0].LeftWheelCollider.sidewaysFriction.stiffness;
    }

    protected virtual void FixedUpdate()
    {
        float forwardMotor = _maxTorque * _fowardThrottle;
        float reverseMotor = _maxTorque * _reverseThrottle;
        _deltaMotorForce = forwardMotor - reverseMotor;

        float braking = 0f;

        _currentSpeed = _rigidbody.velocity.magnitude;
        if (_currentSpeed < 0.01f)
        {
            _currentSpeed = 0f;
        }

        // cap speed
        if (_currentSpeed > _maxSpeed)
        {
            _deltaMotorForce = 0f;
        }

        // adjust max steering over time
        _currentMaxSteerAngle = _maxSteerAngle - _currentSpeed;
        _currentSteerAngle = _currentMaxSteerAngle * _steerValue;

        _directionSign = Mathf.Sign(Vector3.Dot(transform.forward, _rigidbody.velocity));

        // apply forces to wheels
        for (int i = 0; i < _axles.Length; ++i)
        {
            _currentAxle = _axles[i];
            if (_currentAxle.Steer)
            {
                _currentAxle.LeftWheelCollider.steerAngle = _currentSteerAngle;
                _currentAxle.RightWheelCollider.steerAngle = _currentSteerAngle;
            }

            if (_currentAxle.Motor)
            {
                _currentAxle.LeftWheelCollider.motorTorque = _deltaMotorForce;
                _currentAxle.RightWheelCollider.motorTorque = _deltaMotorForce;
            }

            if ((forwardMotor > 0f && _currentSpeed * _directionSign < 0f) || (reverseMotor > 0f && _currentSpeed * _directionSign > 0f))
            {
                // trying to switch directions (forward/reverse)
                braking = _maxBrake;
            }

            _currentAxle.LeftWheelCollider.brakeTorque = braking;
            _currentAxle.RightWheelCollider.brakeTorque = braking;

            // reflect wheel collider movement onto visual wheels
            ApplyPositionToVisuals(_currentAxle);
        }
    }

    private void ApplyPositionToVisuals(Axle current)
    {
        current.LeftWheelCollider.GetWorldPose(out _activeWheelPos, out _activeWheelRot);
        current.LeftWheelTransform.position = _activeWheelPos;
        current.LeftWheelTransform.rotation = _activeWheelRot;

        current.RightWheelCollider.GetWorldPose(out _activeWheelPos, out _activeWheelRot);
        current.RightWheelTransform.position = _activeWheelPos;
        current.RightWheelTransform.rotation = _activeWheelRot;
    }

    private void ChangeSidewaysWheelFriction(float value)
    {
        WheelFrictionCurve curve = _axles[0].LeftWheelCollider.sidewaysFriction;
        curve.stiffness = value;
        //for (int i = 0; i < _axles.Length; ++i)
        //{
            _currentAxle = _axles[1]; // just back wheels for now
            _currentAxle.LeftWheelCollider.sidewaysFriction = curve;
            _currentAxle.RightWheelCollider.sidewaysFriction = curve;
        //}
    }
}
