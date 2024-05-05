using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;
    private bool _entityWasTouchingGround = false;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    [Header("Jump Buffer")]
    [SerializeField] private float _jumpBufferDuration = 0.2f;
    private float _jumpBufferTimer = 0f;

    [Header("Coyote Time")]
    [SerializeField] private float _coyoteTimeDUration = 0.2f;
    private float _coyoteTimeCountDown = -1f;
     
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump Buffer Timer = {_jumpBufferTimer}");
        GUILayout.Label($"CoyoteTime CountDown = {_coyoteTimeCountDown}");
        GUILayout.EndVertical();
    }

    private void Update()
    {
        _UpdateJumpBuffer();
        if (GetInputDash())
        {
            _entity.ApplyDash();
        }
        else
        {
            _entity.SetMoveDirX(GetInputMoveX());
            
            //coyottime
            if (_EntityHasExitGround())
            {
                _ResetCoyoteTime();
            } else
            {
                _UpdateCoyoteTime();
            }
        }

        if (_GetInputDownJump()) {
            if((_entity.IsTouchingGround || _IsCoyoteTimeActive()) && !_entity.IsJumping) {
                _entity.JumpStart();
            } else
            {
                _ResetJumpBuffer();
            }
        }

        if (_entity.IsJumpImpulsing) {
            if (!_GetInputJump() && _entity.IsJumpMinDurationReached)
            {
                _entity.StopJumpImpulsion();
            }
        }

        if (IsJumpBufferActive())
        {
            if ((_entity.IsTouchingGround || _IsCoyoteTimeActive()) && _entity.IsJumping) {
                _entity.JumpStart();
            }
            // coyotetime entityWasTouching
            _entityWasTouchingGround = _entity.IsTouchingGround;
        }
    }

    private bool _GetInputDownJump() {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private float GetInputMoveX()
    {
        float inputMoveX = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            inputMoveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveX = 1f;
        }
        return inputMoveX;
    }


    private bool GetInputDash()
    {
        bool inputMoveDash = false;
        if (Input.GetKey(KeyCode.E))
        {
            inputMoveDash = true;
        }
        return inputMoveDash;
    }

    private bool _GetInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }

    private void _ResetJumpBuffer()
    {
        _jumpBufferTimer = 0f;
    }

    private bool IsJumpBufferActive()
    {
        return _jumpBufferTimer < _jumpBufferDuration;
    }

    private void _UpdateJumpBuffer()
    {
        if (!IsJumpBufferActive()) return;
        _jumpBufferTimer += Time.deltaTime;
    }

    private void _CancelJumpBuffer()
    {
        _jumpBufferTimer = _jumpBufferDuration;
    }

    private void Start()
    {
        _CancelJumpBuffer();
    }

    private void _UpdateCoyoteTime()
    {
        if (!_IsCoyoteTimeActive()) return;
        _coyoteTimeCountDown -= Time.deltaTime;
    }

    private void _ResetCoyoteTime()
    {
        _coyoteTimeCountDown = _coyoteTimeDUration;
    }

    private bool _IsCoyoteTimeActive()
    {
        return _coyoteTimeCountDown > 0f;
    }

    private bool _EntityHasExitGround()
    {
        return _entityWasTouchingGround && !_entity.IsTouchingGround;
    }
}

