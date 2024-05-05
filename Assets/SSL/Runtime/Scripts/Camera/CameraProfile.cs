﻿using UnityEngine;

public class CameraProfile : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private CameraProfileType _profileType = CameraProfileType.Static;

    [Header("Follow")]
    [SerializeField] private CameraFollowable _targetToFollow = null;

    [Header("Damping")]
    [SerializeField] private bool _useDampingHorizontally = false;
    [SerializeField] private float _horizontalDampingFactor = 5f;
    [SerializeField] private bool _useDampingVertically = false;
    [SerializeField] private float _verticalDampingFactor = 5f;

    [Header("Bounds")]
    [SerializeField] private bool _hasBounds = false;
    [SerializeField] private Rect _boundsRect = new Rect(0f, 0f, 10f, 10f);

    public bool HasBounds => _hasBounds;

    public Rect BoundsRect => _boundsRect;

    public bool UseDampingHorizontally => _useDampingHorizontally;

    public float HorizontalDampingFactor => _horizontalDampingFactor;

    public bool UseDampingVertically => _useDampingVertically;

    public float VerticalDampingFactor => _verticalDampingFactor;


    public CameraProfileType ProfileType => _profileType;

    public CameraFollowable TargetToFollow => _targetToFollow;

    private Camera _camera;

    public float CameraSize => _camera.orthographicSize;

    public Vector3 Position => _camera.transform.position;

    private void Awake()
    {

        _camera = GetComponent<Camera>();
        if (_camera != null)
        {
            _camera.enabled = false;

        }
    }


    private void OnDrawGizmosSelected()
    {
        if (!_hasBounds) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_boundsRect.center, _boundsRect.size);
    }
}

public enum CameraProfileType
{
    Static = 0,
    FollowTarget
}