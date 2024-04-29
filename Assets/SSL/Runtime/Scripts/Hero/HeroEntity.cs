using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [SerializeField] private HeroHorizontalMovementsSettings _movementsSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Dash Movements")]
    [SerializeField] private HeroHorizontalDashSettings _DashSettings;
    private bool _isDashMovements = false;
    private float _dashStartTime = 0f;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;


    private void Awake()
    {
      //  _movementsSettings = new HeroHorizontalMovementsSettings();
      //   _DashSettings = new HeroHorizontalDashSettings();
    }

    private void _TurnBack()
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

    private void _Accelerate()
    {
        _horizontalSpeed += _movementsSettings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > _movementsSettings.speedMax) { 
            _horizontalSpeed = _movementsSettings.speedMax;
        }
    }

    private void _Decelerate()
    {
        _horizontalSpeed -= _movementsSettings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f) {
            _horizontalSpeed = 0f;
        }
    }

    private void _UpdateHorizontalSpeed()
    {
        if (_moveDirX != 0f) {
            _Accelerate();
        } else {
            _Decelerate();
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
        if (_isDashMovements && Time.time >= _dashStartTime + _DashSettings.Duration)
        {
            _isDashMovements = false;
            _dashStartTime = 0f;
        }
        
        if (_isDashMovements) {
            _ApplyHorizontalDashSpeed();
        } else
        {
            if (_AreOrientAndMovementOpposite()) {
                _TurnBack();
            } else {
                _UpdateHorizontalSpeed();
                _ChangeOrientFromHorizontalMovement();
            }
            _ApplyHorizontalSpeed();
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

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
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
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.EndVertical();
    }
}
