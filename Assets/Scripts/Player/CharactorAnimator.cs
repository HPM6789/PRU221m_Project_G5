using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List <Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> surfSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    public FacingDirection DefaultDirection { get { return defaultDirection; } }
    //Parameter
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsSurfing { get; set; }

    //States
    SpriteAnimator walkDownAnimator;
    SpriteAnimator walkUpAnimator;
    SpriteAnimator walkRightAnimator;
    SpriteAnimator walkLeftAnimator;

    SpriteAnimator currentAnimator;

    //Ref
    SpriteRenderer spriteRenderer;

    private bool wasPreviousMoving;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkDownAnimator = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnimator = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnimator = new SpriteAnimator(walkRightSprites, spriteRenderer, 0.16f, true);
        walkLeftAnimator = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);
        currentAnimator = walkDownAnimator;
    }

    private void Update()
    {
        var prevAnim = currentAnimator;
        if (!IsSurfing)
        {
            if (MoveX == 1)
            {
                currentAnimator = walkRightAnimator;
            }
            else if (MoveX == -1)
            {
                currentAnimator = walkLeftAnimator;
            }
            else if (MoveY == 1)
            {
                currentAnimator = walkUpAnimator;
            }
            else if (MoveY == -1)
            {
                currentAnimator = walkDownAnimator;
            }
            if (currentAnimator != prevAnim || IsMoving != wasPreviousMoving)
            {
                currentAnimator.Start();
            }
            if (IsMoving)
            {
                currentAnimator.HandleUpdate();
            }
            else
            {
                spriteRenderer.sprite = currentAnimator.Frames[0];
            }

        }
        else
        {
            if (MoveX == 1)
            {
                spriteRenderer.sprite = surfSprites[2];
                spriteRenderer.flipX = true;
            }
            else if (MoveX == -1)
            {
                spriteRenderer.sprite = surfSprites[3];
                spriteRenderer.flipX = false;
            }
            else if (MoveY == 1)
            {
                spriteRenderer.sprite = surfSprites[1];
                spriteRenderer.flipX = false;
            }
            else if (MoveY == -1)
            {
                spriteRenderer.sprite = surfSprites[0];
                spriteRenderer.flipX = false;
            }
        }
        

        wasPreviousMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        MoveX = 0;
        MoveY = 0;

        if(dir == FacingDirection.Left)
        {
            MoveX = -1;
        }else if(dir == FacingDirection.Right)
        {
            MoveX = 1;
        }else if(dir == FacingDirection.Up){
            MoveY = 1;
        }else if(dir == FacingDirection.Down)
        {
            MoveY = -1;
        }
    }
}

public enum FacingDirection { Up, Down, Left, Right };
