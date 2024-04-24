using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM 
{
    protected Dictionary<int, State> statedic = new Dictionary<int, State>();
    protected State curState = null;


    public void Add(int key, State state)
    {
        if(!statedic.ContainsKey(key))
            statedic.Add(key, state);
    }

    public State GetState(int key)
    {
        State state = null;
        if(statedic.ContainsKey(key))
            state = statedic[key];
        
        return state;
    }

    public State GetCurState()
    {
        return curState;
    }

    public Define.PlayerStateType GetCurStatetype()
    {
        int key = 0;
        foreach(var s in statedic)
        {
            if (s.Value == curState)
                key = s.Key;
        }

        return (Define.PlayerStateType)key;
    }

    public void SetCurState(int key)
    {
        if(curState != null)
            curState.Exit();

        curState = GetState(key);

        if(curState != null)
            curState.Enter();
    }

    public void Update()
    {
        if(curState != null)
            curState.Update();
    }    
    
}
