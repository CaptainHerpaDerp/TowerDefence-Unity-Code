using Core.Character;
using Militia;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Fires projectiles at militia units from a distance.
    /// </summary>
    public class EnemyWitch : Enemy
    {
        [Header("The layer which the militia units are on")]
        [SerializeField] private LayerMask milititaUnitLayer;

        [Header("The projectile to fire at the militia units")]
        [SerializeField] private WitchProjectile projectilePrefab;

        public float rangedAttackRange = 0;
        public float rangedAttackCooldown = 0;
        public float rangedAttackDamage = 0;
        public float rangedAttackTravelTime = 0;

        public MilitiaUnit targetMilitiaUnit = null;

        private bool firingAtTarget = false;

        [SerializeField] private float targetDistanceBuffer = 0.5f;
        [SerializeField] private Transform firePosition;

        private bool hasMilitiaTarget => targetMilitiaUnit != null;

        protected override void Start()
        {
            base.Start();

            StartCoroutine(GetMilitiaTarget());
            StartCoroutine(DoRangedAttacking());
        }

        protected override void Update()
        {
            base.Update();
        }

        private IEnumerator GetMilitiaTarget()
        {
            while (true)
            {
                if (!hasMilitiaTarget)
                {

                    // Find the closest militia unit
                    Collider2D[] militiaUnits = Physics2D.OverlapCircleAll(transform.position, rangedAttackRange, milititaUnitLayer);
                    MilitiaUnit closestMilitiaUnit = null;
                    float closestDistance = Mathf.Infinity;

                    foreach (Collider2D militiaUnitCollider in militiaUnits)
                    {
                        MilitiaUnit militiaUnit = militiaUnitCollider.GetComponent<MilitiaUnit>();
                        float distance = Vector2.Distance(transform.position, militiaUnit.transform.position);

                        if (distance < closestDistance)
                        {
                            closestMilitiaUnit = militiaUnit;
                            closestDistance = distance;
                        }
                    }

                    targetMilitiaUnit = closestMilitiaUnit;

                    yield return new WaitForSeconds(0.5f);
                }

                // Check if the target is still valid
                else
                {
                    // Remove target if it is dead, null or out of range
                    if (targetMilitiaUnit == null || targetMilitiaUnit.IsDead() || !MilitiaTargetInRange())
                    {
                        targetMilitiaUnit = null;
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// If a militia target is available, fire a projectile at it.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoRangedAttacking()
        {
            while (true)
            {
                if (!HasMilitiaTarget())
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }

                // Get the angle between the witch and the militia unit
                Vector2 direction = targetMilitiaUnit.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Play the attack animation
                DoAttackAnimation(angle);

                // Before firing, wait for the attack mark time (when the animation is at the point of firing)
                yield return new WaitForSeconds(attackHitMark);

                if (!HasMilitiaTarget())
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }

                // Fire the projectile
                WitchProjectile projectile = Instantiate(projectilePrefab, firePosition.position, Quaternion.identity);
                projectile.SetDamage(rangedAttackDamage);
                projectile.SetTarget(firePosition.position, targetMilitiaUnit.transform, targetMilitiaUnit.transform.position);

                yield return new WaitForSeconds(rangedAttackCooldown);
            }
        }

        private bool MilitiaTargetInRange()
        {
            if (Vector2.Distance(transform.position, targetMilitiaUnit.transform.position) <= rangedAttackRange + targetDistanceBuffer)
            {
                return true;
            }

            return false;
        }

        private bool HasMilitiaTarget()
        {
            return targetMilitiaUnit != null && !targetMilitiaUnit.IsDead() && MilitiaTargetInRange();
        }

        #region Overridden Methods

        protected override void MoveToTargetPosition()
        {
            if (HasMilitiaTarget() || IsDead() || movementTargetPos == Vector3.zero || State == CharacterState.Attacking)
            {
                return;
            }

            // Move towards the waypoint
            Vector3 direction = (movementTargetPos - transform.position).normalized;

            // Velocity based movement
            velocity += direction * speed * Time.deltaTime;

            // Apply damping to simulate sliding
            velocity *= dampingFactor;

            transform.position += velocity;
        }

        #endregion
    }
}