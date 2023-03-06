using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MoveActorAction : CutsenceAction
{
    [SerializeField] CutsenceActor actor;
    [SerializeField] List<Vector2> movePatterns;

    public override IEnumerator Play()
    {
        var charactor = actor.GetCharactor();
        foreach(var moveVec in movePatterns)
        {
           yield return charactor.Move(moveVec, checkCollisions: false);
        }
    }

}

[System.Serializable]
public class CutsenceActor
{
    [SerializeField] bool isPlayer;
    [SerializeField] Charactor charactor;

    public Charactor GetCharactor() => (isPlayer) ? PlayerController.Instance.Charactor : charactor;
}
