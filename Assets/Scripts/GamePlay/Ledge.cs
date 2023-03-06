using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    [SerializeField] int xDir;
    [SerializeField] int yDir;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    public bool TryToJump(Charactor charactor, Vector2 moveDir)
    {
        if(moveDir.x == xDir && moveDir.y == yDir)
        {
            StartCoroutine(Jump(charactor));
            return true;
        }

        return false;
    }

    IEnumerator Jump(Charactor charactor)
    {
        GameController.Instance.PauseGame(true);

       var jumpDest =  charactor.transform.position + new Vector3(xDir, yDir) * 2;

       yield return charactor.transform.DOJump(jumpDest, 0.3f, 1, 0.5f).WaitForCompletion();

        GameController.Instance.PauseGame(false);
    }
}
