using System;

public class PlayerInput
{
    public event Action OnEnable;
    public event Action OnDisable;
    public bool Enable { get => _enable; set => SetState(value); }

    protected readonly PlayerActions _playerActions;
    protected readonly IControllable _controllable;
    private bool _enable = false;

    public PlayerInput(IControllable controllable)
    {
        _playerActions = new();
        _controllable = controllable;
        BindControls();
        BindStateChange();
    }

    private void BindControls()
    {
        _playerActions.Game.Move.started += (ctx) => _controllable.Move = ctx.ReadValue<float>();
        _playerActions.Game.Move.canceled += (ctx) => _controllable.Move = 0f;
        _playerActions.Game.Jump.started += (ctx) => _controllable.Jump = true;
        _playerActions.Game.Jump.canceled += (ctx) => _controllable.Jump = false;
    }

    private void BindStateChange()
    {
        OnEnable += _playerActions.Game.Enable;
        OnDisable += _playerActions.Game.Disable;
    }

    private void SetState(bool enable)
    {
        if (enable == _enable) { return; }
        _enable = enable;
        if (enable) { OnEnable?.Invoke(); } else { OnDisable?.Invoke(); }
    }
}
