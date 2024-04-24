using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    PlayerController player;
    Define.PlayerStateType stateType;
    public Define.PlayerStateType StateType { get { return stateType;}}

    public PlayerState(PlayerController player, Define.PlayerStateType stateType)
    {
        this.player = player;
        this.stateType = stateType;
    }

    public delegate void StateDelegate();
    public StateDelegate OnEnterDelegate {get; set;} = null;
    public StateDelegate OnExitDelegate { get; set;} = null;
    public StateDelegate OnUpdateDelegate { get; set;} = null;  

    public override void Enter()
    {
        OnEnterDelegate?.Invoke();
    }
    public override void Exit()
    {
        OnExitDelegate?.Invoke();
    }
    public override void Update()
    {
        OnUpdateDelegate?.Invoke();
    }
}
