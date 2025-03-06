using System.Collections;
using UnityEngine;
using Core.Character;

namespace Enemies
{
    /// <summary>
    /// A mounted orc enemy that charges at militia units and deals damage
    /// </summary>
    public class MountedOrc : Enemy
    {
        private bool charging = false;

        [HideInInspector] public float defaultSpeed, chargeSpeed, chargeDamage, timeBeforeCharge;

        private const string CHARGEUP = "ChargeUp", CHARGEDOWN = "ChargeDown", CHARGELEFT = "ChargeLeft", CHARGERIGHT = "ChargeRight";

        protected override void Start()
        {
            base.Start();
            defaultSpeed = movementSpeed;
            StartCoroutine(Charge());
        }

        private void DisableCharging()
        {
            charging = false;
            movementSpeed = defaultSpeed;
        }

        /// <summary>
        /// Charges after a certain amount of time has passed and the character is not in combat
        /// </summary>
        /// <returns></returns>
        private IEnumerator Charge()
        {
            while (true)
            {
                if (!charging)
                {
                    if (State == CharacterState.Dead)
                    {
                        yield break;
                    }

                    else if (State == CharacterState.Attacking || HasCombatTarget())
                    {
                        yield return new WaitForSeconds(0.5f);
                    }

                    // Do the charge check
                    if (ObjVelocity != Vector3.zero)
                    {
                        yield return new WaitForSeconds(timeBeforeCharge);

                        if (State == CharacterState.Dead)
                        {
                            yield break;
                        }

                        if (State != CharacterState.Attacking || HasCombatTarget())
                        {
                            movementSpeed = chargeSpeed;
                            charging = true;
                        }
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        #region Overridden Methods

        ///// <summary>
        ///// Overrides the base class method to deal charge damage to the target when it is reached
        ///// </summary>
        ///// <returns></returns>
        protected override IEnumerator AttackTarget()
        {
            while (HasCombatTarget() && !IsDead())
            {
                if (IsDead()) // Check if the character is dead and exit the coroutine if true
                {
                    yield break;
                }

                // Move towards the combat target
                if (Vector3.Distance(transform.position, combatTarget.transform.position) > attackRange)
                {
                    State = CharacterState.Normal;
                    movementTargetPos = combatTarget.transform.position;
                    yield return new WaitForSeconds(0.1f);
                }

                else
                {
                    // If we have reached the target, initiate the attack
                    if (charging && HasCombatTarget())
                    {
                        // Disable charging and reset the speed
                        DisableCharging();

                        // Deal charge damage to the combat target
                        combatTarget.IntakeDamage(chargeDamage);

                        // Play the charge hit sound
                        PlayChargeHitSound();

                        velocity = Vector3.zero;
                        yield return new WaitForSeconds(attackSpeed);
                        continue;
                    }

                    State = CharacterState.Attacking;

                    // Within attack range, initiate attack
                    Vector3 direction = (combatTarget.transform.position - transform.position).normalized;
                    float angle = Vector2.SignedAngle(Vector2.right, direction);

                    DoAttackAnimation(angle);

                    yield return new WaitForSeconds(attackHitMark);

                    PlayAttackSound();

                    if (HasCombatTarget())
                        combatTarget.IntakeDamage(damage);

                    // If target is still not dead, wait for the attack speed and attack again
                    if (HasCombatTarget())
                    {
                        yield return new WaitForSeconds(attackSpeed);
                    }

                    // Otherwise, break the loop
                    else
                    {
                        break;
                    }
                }
            }

            if (IsDead())
            {
                yield break;
            }

            // If the loop exits, the character is not attacking or the target is not valid

            ExitAttackState();
        }

        /// <summary>
        /// Overrides the base class method to play a charging animation when the character is charging
        /// </summary>
        protected override void DoWalkAnimation()
        {
            if (visualAnimator == null)
            {
                return;
            }

            if (State == CharacterState.Dead || State == CharacterState.Attacking || velocity == Vector3.zero)
            {
                visualAnimator.speed = 1;
                return;
            }

            visualAnimator.speed = runAnimationSpeed;

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

                    if (charging)
                    {
                        SetAnimationState(CHARGEUP);
                    }
                    else
                    {
                        SetAnimationState(WALK_UP);
                    }
                }
                else if (angle >= 135 || angle < -135)
                {
                    viewDirection = ViewDirection.Left;

                    if (charging)
                    {
                        SetAnimationState(CHARGELEFT);
                    }
                    else
                    {
                        SetAnimationState(WALK_LEFT);
                    }
                }
                else if (angle >= -135 && angle < -45)
                {
                    viewDirection = ViewDirection.Down;

                    if (charging)
                    {
                        SetAnimationState(CHARGEDOWN);
                    }
                    else
                    {
                        SetAnimationState(WALK_DOWN);
                    }
                }
                else if (angle >= -45 && angle < 45)
                {
                    viewDirection = ViewDirection.Right;

                    if (charging)
                    {
                        SetAnimationState(CHARGERIGHT);
                    }
                    else
                    {
                        SetAnimationState(WALK_RIGHT);
                    }
                }
            }
        }

        #endregion

        #region Audio Methods

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.mountedOrcDeathSound, transform.position);
        }

        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.mountedOrcAttackSound, transform.position);
        }

        protected void PlayChargeHitSound()
        {
            audioManager.PlayOneShot(fmodEvents.mountedOrcChargeHitSound, transform.position);
        }

        #endregion
    }
}