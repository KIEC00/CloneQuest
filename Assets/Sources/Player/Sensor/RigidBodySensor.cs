using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidBodySensor : Sensor
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private ContactFilter2D _contactFilter;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _distance;

    public override int HitCount => GetHits().Length;
    public override RaycastHit2D Hit => HitCount > 0 ? _lastHits[0] : new RaycastHit2D();
    public override ReadOnlyCollection<RaycastHit2D> Hits => Array.AsReadOnly(GetHits());

    private RaycastHit2D[] _lastHits = new RaycastHit2D[0];
    private float _lastUpdated = 0f;

    private bool TimeUpdate()
    {
        var time = Time.fixedTime;
        if (time <= _lastUpdated) { return false; }
        _lastUpdated = time;
        return true;
    }

    private RaycastHit2D[] GetHits()
    {
        if (TimeUpdate()) { CastUpdate(); }
        return _lastHits;
    }

    [ContextMenu("Update")]
    private void CastUpdate()
    {
        var castResults = new List<RaycastHit2D>();
        var count = _rigidbody.Cast(_direction, _contactFilter, castResults, _distance);
        _lastHits = castResults
            .OrderBy(cast => Vector2.Angle(cast.normal, Vector2.up))
            .ToArray();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        _direction.Normalize();
        if (_direction.sqrMagnitude == 0f) { Debug.LogWarning("Direction was not set"); }
        if (_distance == 0f) { Debug.LogWarning("Distance was not set"); }
        if (!_rigidbody) { _rigidbody = GetComponent<Rigidbody2D>(); }
    }

    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject || !enabled) return;
        Gizmos.color = HitCount > 0 ? Color.yellow : Color.grey;
        Gizmos.DrawRay(_rigidbody.position, _direction * _distance);
    }
#endif
}
