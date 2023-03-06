using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorAction : CutsenceAction
{
    [SerializeField] CutsenceActor actor;
    [SerializeField] FacingDirection direction;

    public override IEnumerator Play()
    {
        actor.GetCharactor().Animator.SetFacingDirection(direction);
        yield break;
    }
}
