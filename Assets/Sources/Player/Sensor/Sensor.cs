using System.Collections.ObjectModel;
using UnityEngine;

public abstract class Sensor : MonoBehaviour
{
    public abstract int HitCount { get; }
    public abstract RaycastHit2D Hit { get; }
    public abstract ReadOnlyCollection<RaycastHit2D> Hits { get; }
}
