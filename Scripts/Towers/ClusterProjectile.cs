using System.Collections;
using UnityEngine;
using Core;
using Core.Character;

namespace Towers
{
    /// <summary>
    /// A projectile that explodes on impact and deals damage to all enemies within the explosion radius
    /// </summary>
    public class ClusterProjectile : Projectile
    {
        [SerializeField]
        private GameObject
            surfaceExplosionPrefab,
            waterExplosionPrefab;

        private IEnumerator explosionRoutine = null;

        [SerializeField] private float explosionRadius = 0.2f;
        [SerializeField] private float maxOffset = 0.5f;
        [SerializeField] private float speedOffset;

        public void SetTarget(Vector3 pSourcePosition, Transform pTarget, Vector3 pPreferredTargetPosition)
        {
            sourcePos = pSourcePosition;
            targetPos = pPreferredTargetPosition;
            target = pTarget;

            StartCoroutine(MoveProjectile());
        }

        protected override IEnumerator MoveProjectile()
        {
            while (!hitTarget)
            {
                targetDistance = Vector3.Distance(sourcePos, targetPos);

                // Introduce maximum and minimum projectile speed based on distance
                float projectileSpeed = Mathf.Lerp(maxProjectileSpeed, minProjectileSpeed, targetDistance / sensitivity);

                projectileSpeed += Random.Range(-speedOffset, speedOffset);

                interpolationValue += (Time.deltaTime * projectileSpeed);

                transform.position = GetPosition(interpolationValue);

                Vector3 nextPos = GetPosition(interpolationValue + Time.deltaTime);
                Vector3 posDelta = nextPos - transform.position;

                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg);

                // Check if the projectile has reached the target
                if (interpolationValue >= 1f)
                {
                    soundEffectManager.PlayCatapultProjectileImpactSound();

                    hitTarget = true;

                    // Get all colliders within the explosion radius using the circle2d collider
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

                    if (colliders.Length > 0)
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            if (colliders[i].gameObject != gameObject && colliders[i].TryGetComponent<Character>(out var enemy))
                            {
                                // Calculate a damage multiplier based on the distance from the collision point
                                float distanceMultiplier = Mathf.Lerp(0, 1, targetDistance / maxOffset);

                                // Apply damage with the calculated multiplier
                                float damage = projectileDamage * distanceMultiplier;
                                enemy.IntakeDamage(damage);
                            }
                        }
                    }

                    if (explosionRoutine == null)
                    {
                        explosionRoutine = Explode();
                        StartCoroutine(explosionRoutine);
                    }

                    // Hide the projectile
                    GetComponent<SpriteRenderer>().enabled = false;
                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected override Vector3 GetPosition(float pInterpolationValue)
        {
            // Introduce maximum and minimum arc height based on distance
            float arcHeight = Mathf.Lerp(minArcHeight, maxArcHeight, targetDistance / sensitivity);

            float height = Mathf.Sin(Mathf.PI * pInterpolationValue) * arcHeight;

            Vector3 pos = Vector3.Lerp(sourcePos, targetPos, pInterpolationValue);
            pos += Vector3.up * height;

            return pos;
        }

        /// <summary>
        /// Explode the projectile and deal damage to all enemies within the explosion radius
        /// </summary>
        private IEnumerator Explode()
        {   
            // Check if the projectile is hitting water
            if (IsOverWater())
            {
                impactEffect = Instantiate(waterExplosionPrefab, transform.position, Quaternion.identity, parent: this.transform);
            }
            else
            {
                impactEffect = Instantiate(surfaceExplosionPrefab, transform.position, Quaternion.identity, parent: this.transform);
            }

            yield return new WaitForSeconds(0.5f);

            ReturnSelf();

            yield break;
        }

        protected override void ReturnSelf()
        {
            // Reset the explosion coroutine
            explosionRoutine = null;

            // Re-Enable the sprite renderer
            GetComponent<SpriteRenderer>().enabled = true;

            base.ReturnSelf();
        }
    }
}

