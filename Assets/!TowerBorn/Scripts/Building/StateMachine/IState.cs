using System;
using UnityEditor;
using UnityEngine;

public interface IState
{
    public BuildingState StateType { get; }
    void Enter();
    void Update();
    void Exit();
}