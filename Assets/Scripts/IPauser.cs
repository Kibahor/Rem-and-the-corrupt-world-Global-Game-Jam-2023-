using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPauser
{
    public void AddPausable(IPausable pausable);
    public void Pause();
    public void UnPause();
}
