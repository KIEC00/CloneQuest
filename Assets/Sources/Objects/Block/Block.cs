using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RigidBodySensor))]
public class Block : MonoBehaviour, IPushing
{
    [SerializeField] protected UnityEvent _onLanding;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected RigidBodySensor _sensor;
    [SerializeField] private ContactFilter2D _pushFilter;
    [SerializeField][Range(0f, 200f)] protected float _deceleration;
    [SerializeField][Range(0f, 90f)] protected float _maxSurfaceAngle;
    [SerializeField][Range(0f, 100f)] protected float _clampVelocity;

    protected Vector2 _groundVelocity;
    protected bool _wasGrounded = true;

    protected void FixedUpdate()
    {
        var frameVelocity = _rigidbody.velocity;
        if (_sensor.HitCount > 0)
        {
            var hit = _sensor.Hit;
            if (!_wasGrounded) { _onLanding.Invoke(); _wasGrounded = true; }
            var groundBody = hit.collider.attachedRigidbody;
            var currentGroundVelocity = groundBody ? groundBody.velocity : Vector2.zero;
            frameVelocity += currentGroundVelocity - _groundVelocity;
            _groundVelocity = currentGroundVelocity;

            var groundNormal = hit.normal;
            var angle = Vector2.Angle(Vector2.up, groundNormal);
            if (angle <= _maxSurfaceAngle)
            {
                if (_pushingVelocity.Count == 0) { frameVelocity = Vector2.MoveTowards(frameVelocity, currentGroundVelocity, _deceleration * Time.fixedDeltaTime); }
                var force = _rigidbody.gravityScale * _rigidbody.mass * Physics2D.gravity.y;
                _rigidbody.AddForce(new Vector2(0, -force) + groundNormal * force);
            }

            frameVelocity = Vector2.ClampMagnitude(frameVelocity - currentGroundVelocity, _clampVelocity) + currentGroundVelocity;

        }
        else if (_wasGrounded)
        {
            _wasGrounded = false;
            _groundVelocity = Vector2.zero;
        }
        _rigidbody.velocity = frameVelocity; // Почему-то эта тварь не работает
        _pushingVelocity.Clear();
    }

    private readonly List<float> _pushingVelocity = new();

    public void Push(float velocity)
    {
        Debug.Log($"Push:  {gameObject.name} -> \t{velocity}");
        _pushingVelocity.Add(velocity);
        PushNext(velocity, new());
    }

    public void PushChain(float velocity, List<Transform> pushed)
    {
        Debug.Log($"Chain: {gameObject.name} -> \t{velocity}");
        _pushingVelocity.Add(velocity);
        PushNext(velocity, pushed);
    }

    private void PushNext(float velocity, List<Transform> pushed)
    {
        if (_sensor.HitCount == 0) { return; }
        var castResult = new List<RaycastHit2D>();
        if (_rigidbody.Cast(_rigidbody.velocity.normalized, _pushFilter, castResult, Physics2D.defaultContactOffset * 2) == 0) { return; }
        IPushing pushing = null;
        var hit = castResult
            .OrderBy(cast => cast.point.y)
            .Where(cast => !pushed.Contains(cast.transform))
            .FirstOrDefault(cast => cast.transform != _sensor.Hit.transform && cast.transform.TryGetComponent<IPushing>(out pushing));
        if (pushing == null) { return; }
        pushed.Add(hit.transform);
        pushing.PushChain(velocity, pushed);
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (_rigidbody == null) { _rigidbody = GetComponent<Rigidbody2D>(); }
        if (_sensor == null) { _sensor = GetComponent<RigidBodySensor>(); }
    }
#endif
}
