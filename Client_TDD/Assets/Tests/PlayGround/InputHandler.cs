using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using Zenject;


public class InputHandler : MonoBehaviour, IDisposable, ITickable
{
    public InputHandler()
    {
        Debug.Log("InputHandler is constructed");
    }
    public void Dispose()
    {
        Debug.Log("InputHandler is disposed");
    }
    public void Tick()
    {
        Debug.Log("InputHandler is ticking");
    }
}