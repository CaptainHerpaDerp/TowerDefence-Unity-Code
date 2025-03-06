using System.Collections;
using UnityEngine;
using Core.Character;
using Sirenix.OdinInspector;

namespace Towers
{
    /// <summary>
    /// A projectile with fireball travel and explosion animations
    /// </summary>
    public class FireballProjectile : Projectile
    {
        [SerializeField] private Animator animator;

        [SerializeField] private bool doSlow;

        // Value multiplied to the hit enemy's speed to slow them down
        [ShowIf("doSlow"), SerializeField] private float slowPercentage;

        // Whether the projectile should slow down the hit enemy

        // Duration of the slow effect
        [ShowIf("doSlow"), SerializeField] private float slowDuration;

        // The explosion duration of the projectile before it is destroyed
        private const float ExplodeTime = 0.4f;

        protected override void Start()
        {
            base.Start();

            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("FireballProjectile: No animator found!");
            }
        }

        /// <summary>
        /// Interpolates the projectile's position based on the target's position
        /// </summary>
        protected override IEnumerator MoveProjectile()
        {
            while (!hitTarget)
            {
                if (target == null)
                {
                    targetDistance = Vector3.Distance(sourcePos, targetPos);
                }
                else
                {
                    targetDistance = Vector3.Distance(sourcePos, target.transform.position);
                }

                float projectileSpeed = Mathf.Lerp(maxProjectileSpeed, minProjectileSpeed, targetDistance / sensitivity);

                interpolationValue += (Time.deltaTime * projectileSpeed);

                transform.position = GetPosition(interpolationValue);

                Vector3 nextPos = GetPosition(interpolationValue + Time.deltaTime);

                Vector3 posDelta = nextPos - transform.position;

                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg);

                if (interpolationValue >= 1f)
                {
                    HandleProjectileCollision();
                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        /// <summary>
        /// Deals damage to the target and triggers the explosion animation
        /// </summary>
        private void HandleProjectileCollision()
        {
            if (target != null)
            {
                Character enemy = target.GetComponent<Character>();

                if (!hitTarget && enemy != null)
                {
                    hitTarget = true;
                    enemy.IntakeDamage(projectileDamage);

                    if (doSlow)
                        enemy.ApplySlowEffect(slowPercentage, slowDuration);
                }

            }
      
            ExplodeFireball();       
        }

        private void ExplodeFireball()
        {
            animator.SetTrigger("Explode");

            PlayImpactSound();

            StartCoroutine(DestroyObject());
        }

        private IEnumerator DestroyObject()
        {
            yield return new WaitForSeconds(ExplodeTime);

            ReturnSelf();

            yield return null;
        }

        protected override void PlayImpactSound()
        {
            audioManager.PlayOneShot(fmodEvents.mageFireballHitSound, transform.position);
        }
    }
}
