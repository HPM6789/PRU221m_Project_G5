using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAction : CutsenceAction
{
    [SerializeField] float duration;

    public override IEnumerator Play()
    {
        yield return Fader.Instance.FadeOut(duration);
    }
}
