using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RigidBodySensor))]
public class Block : MonoBehaviour
{
    [SerializeField] protected UnityEvent _onLanding;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected RigidBodySensor _sensor;
    [SerializeField][Range(0f, 200f)] protected float _deceleration;
    [SerializeField][Range(0f, 90f)] protected float _maxSurfaceAngle;
    [SerializeField] protected PhysicsMaterial2D _minFriction;
    [SerializeField] protected PhysicsMaterial2D _maxFriction;

    protected Vector2 _groundVelocity;
    protected bool _wasGrounded = true;

    protected void FixedUpdate()
    {
        var frameVelocity = _rigidbody.velocity;
        var hit = _sensor.Hit;
        var groundCollider = hit.collider;
        _rigidbody.sharedMaterial = _minFriction;
        if (groundCollider)
        {
            if (!_wasGrounded) { _onLanding.Invoke(); _wasGrounded = true; }
            var groundBody = groundCollider.attachedRigidbody;
            var currentGroundVelocity = groundBody ? groundBody.velocity : Vector2.zero;
            frameVelocity += currentGroundVelocity - _groundVelocity;
            _groundVelocity = currentGroundVelocity;

            var groundNormal = _sensor.Hit.normal;
            var angle = Vector2.Angle(Vector2.up, groundNormal);
            if (angle <= _maxSurfaceAngle)
            {
                frameVelocity = Vector2.MoveTowards(frameVelocity, currentGroundVelocity, _deceleration * Time.fixedDeltaTime);
                if ((frameVelocity - currentGroundVelocity).sqrMagnitude == 0f) { _rigidbody.sharedMaterial = _maxFriction; }
            }
        }
        else if (_wasGrounded)
        {
            _wasGrounded = false;
            _groundVelocity = Vector2.zero;
        }
        _rigidbody.velocity = frameVelocity;
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (_rigidbody == null) { _rigidbody = GetComponent<Rigidbody2D>(); }
        if (_sensor == null) { _sensor = GetComponent<RigidBodySensor>(); }
    }
#endif
}
