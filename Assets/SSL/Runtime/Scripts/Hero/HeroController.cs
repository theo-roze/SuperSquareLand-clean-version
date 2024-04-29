using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.EndVertical();
    }

    private void Update()
    {
        if (GetInputDash())
        {
            _entity.ApplyDash();
        }
        else
        {
            _entity.SetMoveDirX(GetInputMoveX());
        }
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

}
