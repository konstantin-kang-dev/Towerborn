
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class CombatStateMachine
{
    private IState _currentState;

    public IState CurrentState => _currentState;

    Dictionary<BuildingState, IState> _states = new Dictionary<BuildingState, IState>();

    public void AddState(IState state)
    {
        _states.Add(state.StateType, state);
    }
    public void ChangeState(BuildingState stateType)
    {
        if (!_states.ContainsKey(stateType)) return;
        IState state = _states[stateType];

        if(_currentState == state) return;

        _currentState?.Exit();
        _currentState = state;
        _currentState?.Enter();
    }

    public void UpdateTick()
    {
        _currentState?.Update();
    }
}