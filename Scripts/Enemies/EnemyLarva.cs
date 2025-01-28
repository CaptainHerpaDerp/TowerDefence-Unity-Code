using Core.Character;
using System.Collections;
using UnityEngine;

namespace Enemies
{

    /// <summary>
    /// An enemy that, intead of moving constantly, will inch towards its target like a caterpillar
    /// </summary>
    public class EnemyLarva : Enemy
    {
        [SerializeField] protected float remainderDuration;
        [SerializeField] protected float walkAnimationMoveStartPoint;
        [SerializeField] protected float walkAnimationMoveEndPoint;

        [Header("The time after the move order before movement actually starts")]
        [SerializeField] protected float moveStartDelay;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private bool resetAnimation;

        private Coroutine moveCoroutine;

        private bool moving;

        protected override void Start()
        {
            base.Start();
            moveCoroutine = StartCoroutine(DoMoveCycle());
        }

        protected override void Update()
        {
            if (resetAnimation)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(DoMoveCycle());

                RepeatAnimationState("WalkRight");

                resetAnimation = false;
            }

            base.Update();
        }

        /// <summary>
        /// Througout the animation duration, start moving at the start point and stop at the end point
        /// </summary>
        protected virtual IEnumerator DoMoveCycle()
        {
            float baseSpeed = speed;

            speed = 0;

            while (true)
            {
                speed = 0;


                // Don't move if the character is not in a normal state
                if (State != CharacterState.Normal)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                PlayWalkAnimation();

                yield return new WaitForSeconds(walkAnimationMoveStartPoint);

                speed = baseSpeed;

                yield return new WaitForSeconds(walkAnimationMoveEndPoint);

                speed = 0;

                yield return new WaitForSeconds(remainderDuration);
            }
        }

        private void PlayWalkAnimation()
        {
            switch (viewDirection)
            {
                case ViewDirection.Up:
                    RepeatAnimationState(WALK_UP);
                    break;
                case ViewDirection.Left:
                    RepeatAnimationState(WALK_LEFT);
                    break;
                case ViewDirection.Down:
                    RepeatAnimationState(WALK_DOWN);
                    break;
                case ViewDirection.Right:
                    RepeatAnimationState(WALK_RIGHT);
                    break;
            }
        }

        #region Overriden Methods

        protected override void DoWalkAnimation()
        {
            if (State == CharacterState.Dead || State == CharacterState.Attacking || velocity == Vector3.zero)
            {
                animator.speed = 1;
                return;
            }

            animator.speed = runAnimationSpeed;

            float xAxis = 0;
            float yAxis = 0;

            if (ObjVelocity.x > 0)
            {
                xAxis = 1;
            }
            else if (ObjVelocity.x < 0)
            {
                xAxis = -1;
            }

            if (ObjVelocity.y > 0)
            {
                yAxis = 1;
            }

            else if (ObjVelocity.y < 0)
            {
                yAxis = -1;
            }

            if (xAxis != 0 || yAxis != 0)
            {
                Vector2 direction = (Vector2)ObjVelocity.normalized;
                float angle = Vector2.SignedAngle(Vector2.right, new Vector2(direction.x, direction.y));

                if (Time.time - lastDirectionChangeTime < minDirectionChangeTime)
                {
                    return;
                }

                if (angle >= 45 && angle < 135)
                {
                    viewDirection = ViewDirection.Up;
                }
                else if (angle >= 135 || angle < -135)
                {
                    viewDirection = ViewDirection.Left;
                }
                else if (angle >= -135 && angle < -45)
                {
                    viewDirection = ViewDirection.Down;
                }
                else if (angle >= -45 && angle < 45)
                {
                    viewDirection = ViewDirection.Right;
                }

                lastDirectionChangeTime = Time.time;
            }
        }        

        #endregion

    }
}