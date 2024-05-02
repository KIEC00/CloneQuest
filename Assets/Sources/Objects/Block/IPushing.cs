using System.Collections.Generic;
using UnityEngine;

public interface IPushing
{
    void Push(float velocity);
    void PushChain(float velocity, List<Transform> pushed);
}