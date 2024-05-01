using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushingBlock : Block, IPushing
{
    public void Push(float pushingVelocity)
    {
        var velocity = _rigidbody.velocity;
        var direction = MathF.Sign(pushingVelocity);
        if (_sensor.Hit.collider)
        {
            var normal = _sensor.Hit.normal;
            var alongGround = new Vector2(normal.y, -normal.x);
            var alongGroundVelocity = new Vector2(pushingVelocity, alongGround.y * pushingVelocity).magnitude;
            velocity = direction * (alongGroundVelocity + _deceleration * Time.fixedDeltaTime) * alongGround;
        }
        else
        {
            velocity.x = pushingVelocity + direction * _deceleration * Time.fixedDeltaTime;
        }
        PushNext(velocity.x);
        _rigidbody.velocity = velocity;
    }

    private void PushNext(float pushingVelocity)
    {
        var direction = MathF.Sign(pushingVelocity);
        if (direction == 0) { return; }
        var hits = new List<RaycastHit2D>();
        if (_rigidbody.Cast(direction > 0 ? Vector2.right : Vector2.left, hits, MathF.Abs(pushingVelocity) * Time.fixedDeltaTime) == 0) { return; }
        IPushing pushing = null;
        hits.FirstOrDefault((hit) => hit.transform != _sensor.Hit.transform && hit.transform.TryGetComponent(out pushing));
        if (pushing == null) { return; }
        pushing.Push(pushingVelocity);
    }
}