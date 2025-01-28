
using Core.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{

    /// <summary>
    /// Angler that has hybrid movement between water and land. Checks the terrain under it to determine if it should be swimming or walking.
    /// </summary>
    public class EnemyAngler : Enemy
    {
        private bool isSwimming = true;
        [SerializeField] private float swimStateCheckInterval = 0.5f;

        protected const string 
            SWIM_UP = "UpSwim",
            SWIM_DOWN = "DownSwim",
            SWIM_LEFT = "LeftSwim",
            SWIM_RIGHT = "RightSwim",

            DIE_UP_SWIM = "DieUpSwim",
            DIE_DOWN_SWIM = "DieDownSwim",
            DIE_LEFT_SWIM = "DieLeftSwim",
            DIE_RIGHT_SWIM = "DieRightSwim";

        protected Dictionary<ViewDirection, string> SwimDeathAnimationKeyPairs;

        protected override void Start()
        {
            base.Start();

            // By default, the angler does not engage in combat unless out of the water
            EngagesInCombat = false;

            SwimDeathAnimationKeyPairs = new Dictionary<ViewDirection, string>
            {
                {ViewDirection.Up, DIE_UP_SWIM},
                {ViewDirection.Down, DIE_DOWN_SWIM},
                {ViewDirection.Left, DIE_LEFT_SWIM},
                {ViewDirection.Right, DIE_RIGHT_SWIM}
            };
        }

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

                if (angle >= 45 && angle < 135)
                {
                    viewDirection = ViewDirection.Up;

                    if (isSwimming)
                    {
                        SetAnimationState(SWIM_UP);
                    }
                    else
                    {
                        SetAnimationState(WALK_UP);
                    }
                }
                else if (angle >= 135 || angle < -135)
                {
                    viewDirection = ViewDirection.Left;
                    
                    if (isSwimming)
                    {
                        SetAnimationState(SWIM_LEFT);
                    }
                    else
                    {
                        SetAnimationState(WALK_LEFT);
                    }
                }
                else if (angle >= -135 && angle < -45)
                {
                    viewDirection = ViewDirection.Down;
                    
                    if (isSwimming)
                    {
                        SetAnimationState(SWIM_DOWN);
                    }
                    else
                    {
                        SetAnimationState(WALK_DOWN);
                    }
                }
                else if (angle >= -45 && angle < 45)
                {
                    viewDirection = ViewDirection.Right;
                    
                    if (isSwimming)
                    {
                        SetAnimationState(SWIM_RIGHT);
                    }
                    else
                    {
                        SetAnimationState(WALK_RIGHT);
                    }
                }
            }
        }

        /// <summary>
        /// Same as default death state, but plays a swimming death animation if the enemy is swimming.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator EnterDeathState()
        {
            velocity = Vector3.zero;

            PlayDeathSound();

            healthBar.Hide();

            eventBus.Publish("EnemyKilled", this);

            State = CharacterState.Dead;

            combatTarget = null;

            if (isSwimming)
            {
                SetAnimationState(SwimDeathAnimationKeyPairs[viewDirection]);
            }
            else
            {
                SetAnimationState(DeathAnimationKeyPairs[viewDirection]);
            }

            yield return new WaitForSeconds(deathAnimationLength);

            Destroy(gameObject);

            yield break;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Do not check for a surface switch if the enemy is not swimming
            if (!isSwimming)
            {
                return;
            }

            if (collision.gameObject.CompareTag("SurfaceSwitch"))
            {
                isSwimming = false;
                EngagesInCombat = true;
                visualTransform.GetComponent<SpriteRenderer>().sortingLayerName = "Actor";
            }
        }

        protected override void PlayAttackSound()
        {
            soundEffectManager.PlayAnglerAttackSound();
        }

        protected override void PlayDeathSound()
        {
            soundEffectManager.PlayAnglerDamageSound();
        }
    }
}