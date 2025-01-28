using System.Collections;
using UnityEngine;
using Core;

namespace Core.Character
{
    /// <summary>
    /// A gameobject that travels from a source to a target position using a modifiable arc trajectory
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        protected Vector3 sourcePos, targetPos;
        protected Transform target;

        protected float interpolationValue = 0f;
        [SerializeField] protected float maxArcHeight = 3f;
        [SerializeField] protected float minArcHeight = 0.5f;
        [SerializeField] protected float maxProjectileSpeed = 3f;
        [SerializeField] protected float minProjectileSpeed = 0.5f;

        [SerializeField] protected float sensitivity = 10f;

        protected float targetDistance;

        protected bool hitTarget = false;

        protected float projectileDamage = 1f;

        [SerializeField] protected LayerMask waterLayer, roadLayer;

        [Header("The impact effect to be created if the projectile lands in water")]
        [SerializeField] protected GameObject waterImpactEffectPrefab;

        protected GameObject impactEffect;

        // Singleton references
        protected SoundEffectManager soundEffectManager;
        protected EventBus eventBus;

        protected virtual void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;
            eventBus = EventBus.Instance;
        }

        public Transform GetTarget()
        {
            return target;
        }

        public virtual void SetDamage(float amount)
        {
            projectileDamage = amount;
        }

        public virtual void SetTarget(Vector3 pSourcePosition, Transform pTarget, Vector2 pTargetPosition)
        {
            sourcePos = pSourcePosition;
            targetPos = pTargetPosition;

            if (pTarget != null)
            {
                target = pTarget;
                StartCoroutine(MoveProjectile());
            }

            transform.position = sourcePos;
        }

        /// <summary>
        /// Returns true if the projectile is over water
        /// </summary>
        /// <returns></returns>
        protected bool IsOverWater()
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, 0, roadLayer))
            {
                return false;
            }

            return Physics2D.Raycast(transform.position, Vector2.down, 0, waterLayer);
        }

        /// <summary>
        /// While the projectile has not hit the target, move the projectile towards the target. If the interpolation value reaches 1, the projectile has hit the target, the target intakes damage and the projectile is destroyed.
        /// </summary>
        protected virtual IEnumerator MoveProjectile()
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

                // Introduce maximum and minimum projectile speed based on distance
                float projectileSpeed = Mathf.Lerp(maxProjectileSpeed, minProjectileSpeed, targetDistance / sensitivity);

                interpolationValue += (Time.deltaTime * projectileSpeed);

                transform.position = GetPosition(interpolationValue);

                Vector3 nextPos = GetPosition(interpolationValue + Time.deltaTime);

                Vector3 posDelta = nextPos - transform.position;

                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg);

                if (interpolationValue >= 1f)
                {
                    if (target != null)
                    {
                        Character enemy = target.GetComponent<Character>();

                        if (enemy == null || enemy.IsDead())
                        {
                            StopMoving();
                            yield break;
                        }

                        soundEffectManager.PlayHitSound();
                        if (!hitTarget && enemy != null)
                        {
                            hitTarget = true;
                            enemy.IntakeDamage(projectileDamage);
                        }

                        ReturnSelf();
                    }
                    else
                    {
                        StopMoving();
                    }

                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        /// <summary>
        /// Stop interpolating and keep the projectile at the current position
        /// </summary>
        protected virtual void StopMoving()
        {
            hitTarget = true;

            // See if the projectile is over water
            if (IsOverWater())
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = false;

                impactEffect = Instantiate(waterImpactEffectPrefab, transform.position, Quaternion.identity);
                StartCoroutine(DestroyImpactEffect(impactEffect));
            }
            else
            {
                StartCoroutine(FadeAndDestroy());
            }
        }

        protected virtual IEnumerator DestroyImpactEffect(GameObject effectObj)
        {
            yield return new WaitForSeconds(0.5f);

            Destroy(effectObj);
            ReturnSelf();
        }

        protected virtual void ReturnSelf()
        {
            // Reset the arrow
            interpolationValue = 0f;    
            hitTarget = false;

            // Reset the color of the projectile
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

            Destroy(impactEffect);
            eventBus.Publish("ProjectileReturn", this);
        }

        /// <summary>
        /// Fades the projectile out and destroys it
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator FadeAndDestroy()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            yield return new WaitForSeconds(0.5f);

            while (spriteRenderer.color.a > 0)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 0.1f);
                yield return new WaitForSeconds(0.1f);
            }

            ReturnSelf();
        }

        /// <summary>
        /// Get the position of the projectile based on the interpolation value
        /// </summary>
        /// <param name="pInterpolationValue"></param>
        /// <returns></returns>
        protected virtual Vector3 GetPosition(float pInterpolationValue)
        {
            // Introduce maximum and minimum arc height based on distance
            float arcHeight = Mathf.Lerp(minArcHeight, maxArcHeight, targetDistance / sensitivity);

            float height = Mathf.Sin(Mathf.PI * pInterpolationValue) * arcHeight;

            Vector3 pos = Vector3.zero;

            if (target != null)
            {
                pos = Vector3.Lerp(sourcePos, target.transform.position, pInterpolationValue);
            }
            else
            {
                pos = Vector3.Lerp(sourcePos, targetPos, pInterpolationValue);
            }

            pos += Vector3.up * height;

            return pos;
        }
    }
}

