using System.Collections;
using UnityEngine;
using Core.Character;

namespace Enemies
{
    /// <summary>
    /// A larger squid that can preform a single long dash in a level
    /// </summary>
    public class EnemyGiantSquid : EnemySquid
    {
        [Header("Dash Settings")]
        [SerializeField] private float baseDashChance;

        [Header("The increase of likelyhood of dashing after each movement action")]
        [SerializeField] private float dashAdditionPerMove;

        [SerializeField] private float dashMoveStartDelay;

        [SerializeField] private float dashSpeedCutCooldown;

        public float DashSpeed;

        private bool hasDashed = false;
        private float currentDashChance;

        private const string 
            DASH_UP = "SquidDashUp",
            DASH_DOWN = "SquidDashDown",
            DASH_LEFT = "SquidDashLeft",
            DASH_RIGHT = "SquidDashRight";

        protected override void Start()
        {
            base.Start();
            currentDashChance = baseDashChance;
        }

        #region Overridden Methods

        protected override IEnumerator DoSpeedCut()
        {
            float baseSpeed = movementSpeed;

            while (true)
            {

                // Stop moving
                movementSpeed = 0;

                // Get the direction to the next waypoint for the swimming animation
                Vector3 direction = (roadWaypoints[CurrentWaypointIndex] - transform.position).normalized;

                // Do a dash roll to see if the squid should dash (if it hasn't already)
                if (DoDashRoll())
                {
                    PlayDashingAnimation(direction);

                    yield return new WaitForSeconds(dashMoveStartDelay);

                    movementSpeed = DashSpeed;

                    yield return new WaitForSeconds(dashSpeedCutCooldown);
                }

                // Preform the default movement action
                else
                {
                    if (IsDead())
                        yield break;

                    // Increase the dash chance
                    currentDashChance += dashAdditionPerMove;

                    PlaySwimmingAnimation(direction);

                    // Wait for a moment before applying speed (to correspond with the animation)
                    yield return new WaitForSeconds(moveStartDelay);

                    // Apply the base speed
                    movementSpeed = baseSpeed;

                    yield return new WaitForSeconds(speedCutCooldown);
                }
            }
        }

        protected override void DoWalkAnimation()
        {
            return;
        }

        protected override IEnumerator EnterDeathState()
        {
            // print the current view direction
            print(viewDirection);

            // Reset the rotation of the squid
            transform.rotation = Quaternion.identity;
            return base.EnterDeathState();
        }

        #endregion

        private bool DoDashRoll()
        {
            // If the squid has already dashed, it can't dash again
            if (hasDashed)
                return false;

            float dashRoll = Random.Range(0, 100);

            // If the dash roll is less than the base dash chance, the squid will dash
            if (dashRoll < currentDashChance)
            {
                hasDashed = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void PlayDashingAnimation(Vector3 direction)
        {
            float angle = Vector2.SignedAngle(Vector2.right, new Vector2(direction.x, direction.y));

            if (angle >= 45 && angle < 135)
            {
                // Dashing Up
                RepeatAnimationState(DASH_UP);
                horizontalMovement = false;
                viewDirection = ViewDirection.Up;
            }
            else if (angle >= 135 || angle < -135)
            {
                // Dashing Left
                RepeatAnimationState(DASH_LEFT);
                horizontalMovement = true;
                viewDirection = ViewDirection.Left;
            }
            else if (angle >= -135 && angle < -45)
            {
                // Dashing Down
                RepeatAnimationState(DASH_DOWN);
                horizontalMovement = false;
                viewDirection = ViewDirection.Down;
            }
            else if (angle >= -45 && angle < 45)
            {
                // Dashing Right
                RepeatAnimationState(DASH_RIGHT);
                horizontalMovement = true;
                viewDirection = ViewDirection.Right;
            }
        }
    }
}