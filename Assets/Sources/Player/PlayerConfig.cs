using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject
{
    [Header("Moving")]
    [SerializeField] private float _velocity = 15f;
    [SerializeField] private float _acceleration = 100f;
    [SerializeField] private float _deceleration = 70f;
    [SerializeField] private float _airVelocity = 10f;
    [SerializeField] private float _airAcceleration = 80f;
    [SerializeField] private float _airDeceleration = 30f;

    [Header("Jumping")]
    [SerializeField] private float _jumpVelocity = 20f;
    [SerializeField] private float _coyoteTime = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float _maxFallVelocity = 30f;
    [SerializeField] private float _jumpEndEarlyGravityModifier = 3f;
    [SerializeField] private float _maxSurfaceAngle = 65f;

    // Moving Getters
    public float Velocity => _velocity;
    public float Acceleration => _acceleration;
    public float Deceleration => _deceleration;
    public float AirVelocity => _airVelocity;
    public float AirAcceleration => _airAcceleration;
    public float AirDeceleration => _airDeceleration;

    // Jumping Getters
    public float JumpVelocity => _jumpVelocity;
    public float CoyoteTime => _coyoteTime;

    // Gravity Getters
    public float MaxFallVelocity => _maxFallVelocity;
    public float JumpEndEarlyGravityModifier => _jumpEndEarlyGravityModifier;
    public float MaxSurfaceAngle => _maxSurfaceAngle;
}