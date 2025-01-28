using System.Collections;
using UnityEngine;

namespace Core.Character
{
    /// <summary>
    /// A projectile that travels from an enemy to a militia unit
    /// </summary>
    public class WitchProjectile : Projectile
    {
        [SerializeField] private float projectileSpeed = 1f;

        /// <summary>
        /// Stop interpolating and keep the projectile at the current position
        /// </summary>
        protected override void StopMoving()
        {
            hitTarget = true;

            Destroy(gameObject);
        }


        /// <summary>
        /// Moves the projectile towards the target. If it hits the target or reaches the destination, it deals damage and is destroyed.
        /// </summary>
        protected override IEnumerator MoveProjectile()
        {
            while (!hitTarget)
            {
                Vector3 currentTargetPos = target ? target.position : targetPos;

                // Move the projectile towards the target
                transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, projectileSpeed * Time.deltaTime);

                // Rotate the projectile to face the target
                Vector3 direction = (currentTargetPos - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

                // Check if the projectile has reached the target
                if (Vector3.Distance(transform.position, currentTargetPos) <= 0.1f)
                {
                    if (target != null)
                    {
                        Character enemy = target.GetComponent<Character>();

                        if (enemy != null && !enemy.IsDead())
                        {
                            soundEffectManager.PlayHitSound();
                            enemy.IntakeDamage(projectileDamage);
                        }
                    }

                    StopMoving();
                }

                yield return null;
            }
        }
    }
}