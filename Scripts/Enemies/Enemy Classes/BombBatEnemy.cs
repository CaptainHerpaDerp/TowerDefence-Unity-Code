using Core.Character;
using Enemies;
using Militia;
using System.Collections;
using UnityEngine;

/// <summary>
/// An enemy that will explode as soon as it gets close to a militia unit
/// </summary>
public class BombBatEnemy : Enemy
{
    [Header("The layer which the militia units are on")]
    [SerializeField] private LayerMask milititaUnitLayer;

    // The time after the explosion animation starts that the damage is dealt to the militia units
    [SerializeField] private float bombExplosionTime;

    // The total duration of the explosion animation
    [SerializeField] private float explosionAnimationDuration;

    // The range at which the enemy will explode
    public float explosionRadius;

    // The range at which the enemy will detect militia units and start moving towards them
    public float detectionRange;

 //   private float preservedMinDistToTarget;
    [SerializeField] private float minDistToTarget = 0f;

    private static string explosion1 = "Explode1", explosion2 = "Explode2", explosion3 = "Explode3";

    public MilitiaUnit targetMilitiaUnit = null;
    private bool hasMilitiaTarget => targetMilitiaUnit != null || targetMilitiaUnit != null && !targetMilitiaUnit.IsDead();

    protected override void Start()
    {
        base.Start();

        EngagesInCombat = false;

        // Store the current minimum distance to the target
      //  preservedMinDistToTarget = minDistanceToWaypoint;

        StartCoroutine(GetMilitiaTarget());
    }

    private IEnumerator GetMilitiaTarget()
    {
        while (true)
        {
            if (!hasMilitiaTarget)
            {
                // Find the closest militia unit
                Collider2D[] militiaUnits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, milititaUnitLayer);
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

                if (targetMilitiaUnit != null)
                {
                    State = CharacterState.Attacking;
                    movementTargetPos = targetMilitiaUnit.transform.position;

                    // Since the bat is now moving towards the target, we want to reduce the minimum distance to the target.
                  //  minDistanceToWaypoint = minDistToTarget;
                }

                yield return new WaitForSeconds(0.5f);
            }

            else if (hasMilitiaTarget && Vector2.Distance(transform.position, targetMilitiaUnit.transform.position) <= minDistToTarget)
            {
                Debug.Log("contact");

                // The bat has reached the target militia unit, so it should explode
                State = CharacterState.Attacking;

                // Play the explosion animation
                PlayExplosionAnimation();

                // Deal damage to the militia unit
             //   targetMilitiaUnit.TakeDamage(attackDamage);

                // Wait for the explosion animation to finish
                yield return new WaitForSeconds(explosionAnimationDuration);

                // Destroy the bat
                Destroy(gameObject);
            }

            // Check if the target is still valid
            else
            {
                if (hasMilitiaTarget)
                Debug.Log($"Distance: {Vector2.Distance(transform.position, targetMilitiaUnit.transform.position)}");

                // Remove target if it is dead, null or out of range

                Debug.Log("Target is dead");

                // Restore the minimum distance to the target since the bat is no longer moving towards the target
              //  minDistanceToWaypoint = preservedMinDistToTarget;
                targetMilitiaUnit = null;

                State = CharacterState.Normal;

                StartCoroutine(TravelWaypoints());

            }

            yield return new WaitForFixedUpdate();
        }
    }

    protected override void MoveToTargetPosition()
    {
        base.MoveToTargetPosition();
    }

    private void PlayExplosionAnimation()
    {
        // Plays a random explosion animation
    }

    protected override void PlayDeathSound()
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayAttackSound()
    {
        throw new System.NotImplementedException();
    }
}
