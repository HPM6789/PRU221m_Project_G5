using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CutsenceAction
{
    [SerializeField] string name;
    [SerializeField] bool waitForComplete = true;

    public virtual IEnumerator Play()
    {
        yield break;
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public bool WaitForComplete => waitForComplete;
}
