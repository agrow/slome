using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/* This script fixes a bug in Cinemachine's camera controls
 * Zoom is reading scroll X not scroll Y
 * Script from here: https://discussions.unity.com/t/cinemachineinputaxiscontroller-orbitscale-inputsystem-not-work/1569913 
 * NOTE: DID NOT FIX THE BUG
 */

[Serializable]
public class SwapAxisProcessor : InputProcessor<Vector2>
{
    [Tooltip("send x-axis to y-axis and vise versa")]
    public bool swapAxis = true;

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        return swapAxis ? new(value.y, value.x) : value;
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        Debug.Log("Initializing SwapAxisProcessor");
        InputSystem.RegisterProcessor<SwapAxisProcessor>();
    }

}
