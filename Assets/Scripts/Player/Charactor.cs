using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Charactor : MonoBehaviour
{
    CharactorAnimator animator;

    public float moveSpeed;
    bool isMoving;
    public CharactorAnimator Animator { get { return animator; } }
    public bool IsMoving { get; private set; }
    private void Awake()
    {
        animator = GetComponent<CharactorAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + 0.3f;

        transform.position = pos;
    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null, bool checkCollisions = true)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        var ledge = CheckForLedge(targetPos);
        if (ledge != null)
        {
          if(  ledge.TryToJump(this, moveVec))
            {
                yield break;
            }
        }

        if (!IsPathClear(targetPos) && checkCollisions)
            yield break;
        if(animator.IsSurfing && Physics2D.OverlapCircle(targetPos, 0.3f, GameLayer.i.WaterLayer) == null)
        {
            animator.IsSurfing = false;
        }

        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;
        OnMoveOver?.Invoke();
    }

    Ledge CheckForLedge(Vector3 targetPos)
    {
       var collider =  Physics2D.OverlapCircle(targetPos, 0.15f, GameLayer.i.LedgesLayer);
       return collider?.GetComponent<Ledge>();
    }
    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }
    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        var collisionLayer = GameLayer.i.SolidObjectsLayer | GameLayer.i.InteractableLayersLayer | GameLayer.i.PlayerLayer;
        if (!animator.IsSurfing)
            collisionLayer = collisionLayer | GameLayer.i.WaterLayer;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.1f, 0.1f), 0f, dir, diff.magnitude - 1, collisionLayer) == true)
            return false;

        return true;
    }

    public void LookToward(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if(xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
        {

        }
    }
    public bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayer.i.SolidObjectsLayer | GameLayer.i.InteractableLayersLayer) != null)
        {
            return false;
        }
        return true;
    }

    
}
