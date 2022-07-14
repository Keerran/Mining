using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class RepeatInteraction : IInputInteraction
{
    public float startDelay = 0.5f;
    public float repeatDelay = 0.1f;
    private double _performTime;

    #if UNITY_EDITOR
    static RepeatInteraction()
    {
        InputSystem.RegisterInteraction<RepeatInteraction>();
    }
    #endif

    public void Process(ref InputInteractionContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.ControlIsActuated())
                {
                    context.PerformedAndStayStarted();
                    _performTime = context.time + startDelay;
                    context.SetTimeout(startDelay);
                }
                break;
            case InputActionPhase.Started:
                if (context.ControlIsActuated())
                {
                    if (context.time > _performTime)
                    {
                        context.PerformedAndStayStarted();
                        _performTime = context.time + repeatDelay;
                        context.SetTimeout(repeatDelay);
                    }
                }
                else context.Canceled();
                break;
        }
    }

    public void Reset()
    {
        _performTime = 0;
    }
}
