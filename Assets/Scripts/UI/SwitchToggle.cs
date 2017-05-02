using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwitchToggle: MonoBehaviour
{
    public abstract void Load();

    public abstract void Toggle(bool on);
}
