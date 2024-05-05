using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementsSettings")]
    [SerializeField] private HeroHorizontalMovementsSettings _groundHorizontalMovementsSettings;
    [SerializeField] private HeroHorizontalMovementsSettings _airHorizontalMovementsSettings;
    [SerializeField] private HeroHorizontalMovementsSettings _movementsSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; private set; } = false;

    [Header("Dash Movements")]
    [SerializeField] private HeroHorizontalDashSettings _DashSettings;
    [SerializeField] private HeroAirHorizontalDashSettings _DashAirSettings;
    private bool _isDashMovements = false;
    private float _dashStartTime = 0f;

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings _jumpSettings;
    [SerializeField] private HeroFallSettings _jumpFallSettings;
    enum JumpState 
    { 
        NotJumping,
        JumpImpulsion,
        Falling,
    }
    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

   
    private void _TurnBack(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed -= _movementsSettings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f ) { 
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void _Accelerate(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax) { 
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _Decelerate(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f) {
            _horizontalSpeed = 0f;
        }
    }

    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }

    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }

    private void _UpdateHorizontalSpeed(HeroHorizontalMovementsSettings settings)
    {
        if (_moveDirX != 0f) {
            _Accelerate(settings);
        } else {
            _Decelerate(settings);
        }
    }

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    public void ApplyDash()
    {
       if (_isDashMovements)
        {
            return;
        } else
        {
            _isDashMovements = true;
            _dashStartTime = Time.time;
        } 
    }

    private void FixedUpdate()
    {
        _ApplyGroundDetection();
        _UpdateCameraFollowPosition();
        HeroHorizontalMovementsSettings horizontalMovementSettings = _GetCurrentHorizontalMovementSettings();

        if (_isDashMovements && IsTouchingGround && Time.time >= _dashStartTime + _DashSettings.Duration)
        {
            _isDashMovements = false;
            _dashStartTime = 0f;
        }

        if (_isDashMovements && !IsTouchingGround && Time.time >= _dashStartTime + _DashAirSettings.Duration)
        {
            _isDashMovements = false;
            _dashStartTime = 0f;
            _jumpState = JumpState.Falling;
        }

        if (_isDashMovements && IsTouchingGround) {
            _ApplyHorizontalDashSpeed();
        } else if (_isDashMovements && !IsTouchingGround)
        { 
            _ApplyHorizontalDashAirSpeed();
        }
        else  {
            if (_AreOrientAndMovementOpposite())
            {
                _TurnBack(horizontalMovementSettings);
            }
            else
            {
                _UpdateHorizontalSpeed(horizontalMovementSettings);
                _ChangeOrientFromHorizontalMovement();
            }

            if (IsJumping)
            {
                _UpdateJump();
            }
            else
            {
                if (!IsTouchingGround)
                {
                    _ApplyFallGravity(_fallSettings);
                }
                else
                {
                    _ResetVerticalSpeed();
                }
            }

            _ApplyHorizontalSpeed();
            _ApplyVerticalSpeed();
        }
    }

    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _ApplyHorizontalDashSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _DashSettings.Speed * _orientX;
        _rigidbody.velocity = velocity;
    }
    private void _ApplyHorizontalDashAirSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _DashAirSettings.Speed * _orientX;
        velocity.y = 0;
        _rigidbody.velocity = velocity;
        _jumpState = JumpState.NotJumping;
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax) {
            _verticalSpeed = -settings.fallSpeedMax;
        } 
    }

    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _jumpSettings.jumpMaxDuration)
        {
            _verticalSpeed = _jumpSettings.jumpSpeed;
        } else
        {
            _jumpState = JumpState.Falling;
        }
    }

    private void _UpdateJumpStateFalling()
    {
        if (!IsTouchingGround)
        {
            _ApplyFallGravity(_jumpFallSettings);
        } else
        {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion(); 
                break;

            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }

    public void StopJumpImpulsion()
    {
        //_jumpState = _jumpState.Falling;
    }

    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    public bool IsJumpMinDurationReached => _jumpTimer >= _jumpSettings.jumpMinDuration;

    public void JumpStart()
    {
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    private HeroHorizontalMovementsSettings _GetCurrentHorizontalMovementSettings()
    {
        return IsTouchingGround ? _groundHorizontalMovementsSettings : _airHorizontalMovementsSettings;
    }

    public bool IsJumping => _jumpState != JumpState.NotJumping;

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if (IsTouchingGround) {
            GUILayout.Label("OnGround");
        }else
        {
            GUILayout.Label("InAir");
        }
        GUILayout.Label($"JumpState = {_jumpState}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }

    //Camera Follow
    private CameraFollowable _cameraFollowable;

    private void Awake()
    {
        _cameraFollowable = GetComponent<CameraFollowable>();
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        //_cameraFollowable.FollowPositionY = GetComponent<Rigidbody2D>().position.y;
        _cameraFollowable.FollowPositionY = _rigidbody.position.y;
    }

    private void _UpdateCameraFollowPosition()
    {
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        if (IsTouchingGround && !IsJumping)
        {
            _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        }
    }

}
