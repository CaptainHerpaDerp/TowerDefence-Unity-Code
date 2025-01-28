using UnityEngine;

namespace Towers
{
    /// <summary>
    /// Subclass of the Tower Unit, which includes more directional-specific animations
    /// </summary>
    public class CatapultUnit : TowerUnit
    {
        private const string ATTACK_BOTTOM_RIGHT = "BottomRight", ATTACK_BOTTOM_LEFT = "BottomLeft", ATTACK_TOP_RIGHT = "TopRight", ATTACK_TOP_LEFT = "TopLeft";

        public override void InitializeAnimationKeys()
        {
            ATTACK_UP = "Up";
            ATTACK_DOWN = "Down";
            ATTACK_LEFT = "Left";
            ATTACK_RIGHT = "Right";
        }

        /// <summary>
        /// Overrides the base class method to include more more directional-specific animations
        /// </summary>
        /// <param name="target"></param>
        public override void AttackTowards(Transform target)
        {
            if (animator == null)
            {
                Debug.LogError("Animator is not assigned!");
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;
            float angle = Vector2.SignedAngle(Vector2.right, direction);

            // Based on the angle, attack in the corresponding direction, including Up and Down left and right, bottom left, bottom right, top right, top left

            if (angle > 22.5f && angle < 67.5f)
            {
                SetAnimationState(ATTACK_TOP_RIGHT);
            }
            else if (angle > 67.5f && angle < 112.5f)
            {
                SetAnimationState(ATTACK_UP);
            }
            else if (angle > 112.5f && angle < 157.5f)
            {
                SetAnimationState(ATTACK_TOP_LEFT);
            }
            else if (angle > 157.5f || angle < -157.5f)
            {
                SetAnimationState(ATTACK_LEFT);
            }
            else if (angle > -157.5f && angle < -112.5f)
            {
                SetAnimationState(ATTACK_BOTTOM_LEFT);
            }
            else if (angle > -112.5f && angle < -67.5f)
            {
                SetAnimationState(ATTACK_DOWN);
            }
            else if (angle > -67.5f && angle < -22.5f)
            {
                SetAnimationState(ATTACK_BOTTOM_RIGHT);
            }
            else
            {
                SetAnimationState(ATTACK_RIGHT);
            }
        }

        // Because the catapult only has one animation, the same one must often be repeated
        public override void SetAnimationState(string newState)
        {
            animator.CrossFadeInFixedTime(newState, 0.01f);
        }
    }
}