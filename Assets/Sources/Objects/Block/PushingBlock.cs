using System;
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
        _rigidbody.velocity = velocity;
    }
}