using System.Collections;
using UnityEngine;
using Core.Character;

namespace Enemies
{
    public class EnemySquid : Enemy
    {
        [SerializeField] protected float speedCutDuration;
        [SerializeField] protected float speedCutCooldown;

        public float rotationOffset;

        [Header("The time after the move order before movement actually starts")]
        [SerializeField] protected float moveStartDelay;

        [SerializeField] protected float rotationSpeed;
        protected Quaternion targetRotation;

        protected bool horizontalMovement;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(DoSpeedCut());
            StartCoroutine(RotateTowardsTarget());
        }

        protected override void Update()
        {
            base.Update();

            if (movementSpeed > 0)
            {
                RotateTowardsDirection();
            }       
        }

        protected virtual IEnumerator DoSpeedCut()
        {
            float baseSpeed = movementSpeed;

            while (true)
            {
                yield return new WaitForSeconds(speedCutCooldown);

                // Stop moving
                movementSpeed = 0;

                yield return new WaitForSeconds(speedCutDuration);

                if (IsDead())
                    yield break;

                Vector3 direction = (roadWaypoints[CurrentWaypointIndex] - transform.position).normalized;
                PlaySwimmingAnimation(direction);

                yield return new WaitForSeconds(moveStartDelay);

                // Start moving again
                movementSpeed = baseSpeed;
            }
        }

        /// <summary>
        /// Rotates the enemy towards the direction it is moving
        /// </summary>
        protected void RotateTowardsDirection()
        {
            Vector3 direction = movementTargetPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward);
        }

        /// <summary>
        /// Rotates the visualtransform towards the target rotation by a given speed
        /// </summary>
        protected IEnumerator RotateTowardsTarget()
        {
            while (true)
            {
                // if not moving horizontally (playing and up or down animation), reset the rotation
                if (!horizontalMovement)
                {
                    visualTransform.rotation = Quaternion.identity;
                    yield return new WaitForFixedUpdate();
                    continue;
                }

                visualTransform.rotation = Quaternion.RotateTowards(visualTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
        }

        protected virtual void PlaySwimmingAnimation(Vector3 direction)
        {
            float angle = Vector2.SignedAngle(Vector2.right, new Vector2(direction.x, direction.y));

            if (angle >= 45 && angle < 135)
            {
                // Walking Up
                RepeatAnimationState(WALK_UP);
                horizontalMovement = false;
                viewDirection = ViewDirection.Up;
            }
            else if (angle >= 135 || angle < -135)
            {
                // Walking Left
                RepeatAnimationState(WALK_LEFT);
                horizontalMovement = true;
                viewDirection = ViewDirection.Left;
            }
            else if (angle >= -135 && angle < -45)
            {
                // Walking Down
                RepeatAnimationState(WALK_DOWN);
                horizontalMovement = false;
                viewDirection = ViewDirection.Down;
            }
            else if (angle >= -45 && angle < 45)
            {
                // Walking Right
                RepeatAnimationState(WALK_RIGHT);
                horizontalMovement = true;
                viewDirection = ViewDirection.Right;
            }
        }

        // Squid does not have an attack
        protected override void PlayAttackSound() { }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.squidDeathSound, transform.position);
        }
    }
}