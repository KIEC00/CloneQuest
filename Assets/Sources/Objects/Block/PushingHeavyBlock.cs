using System.Collections;
using UnityEngine;

public class PushingHeavyBlock : PushingBlock, IPushing
{
    private int _pushCount;
    private Coroutine _resetRoutine;

    public new void Push(float pushingVelocity)
    {
        if (++_pushCount > 1) { base.Push(pushingVelocity); }
        if (_resetRoutine == null) { StartCoroutine(ResetCounter()); }
    }

    public new void PushChain(float pushingVelocity) { }

    IEnumerator ResetCounter()
    {
        yield return new WaitForFixedUpdate();
        _pushCount = 0;
    }
}