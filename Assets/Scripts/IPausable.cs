using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
    public void InitialisePause(IPauser pauser);
    public void OnPause();
    public void OnUnPause();
}
