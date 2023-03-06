using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAction : CutsenceAction
{
    [SerializeField] float duration;

    public override IEnumerator Play()
    {
        yield return Fader.Instance.FadeIn(duration);
    }
}
