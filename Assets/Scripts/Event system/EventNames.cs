using UnityEngine;
using System.Collections;


/// <summary>
/// Use standard event names for events that don't need parameters sent to them
/// </summary>
public enum StandardEventName
{
    None = 0,

    Pause = 1,
    Unpause = 2,
    PropDeactivated = 3,
}


/// <summary>
/// Use boolean event names for events that need a True or False sent to them
/// </summary>
public enum BooleanEventName
{
    None = 0,

    SwitchTorch = 1,
}


/// <summary>
/// Use float event names that need a float value sent to them
/// </summary>
public enum FloatEventName
{
    None = 0,

}


/// <summary>
/// Use two-floats event names that need two float values sent to them
/// </summary>
public enum TwoFloatsEventName
{
    None = 0,

}


/// <summary>
/// Use integer event names for events that need an integger sent to them
/// </summary>
public enum IntegerEventName
{
    None = 0,

}


/// <summary>
/// Use string event names that need a string sent to them
/// </summary>
public enum StringEventName
{
    None = 0,

}


/// <summary>
/// Use transform event names that need a transform component sent to them
/// </summary>
public enum TransformEventName
{
    None = 0,

    PropActivated = 1,
}
