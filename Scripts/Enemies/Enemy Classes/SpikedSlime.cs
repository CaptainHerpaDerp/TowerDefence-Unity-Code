using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Character;
using Militia;

namespace Enemies
{
    /// <summary>
    /// A slime that has an AOE attack and turn other slimes into spiked slimes when it dies. Cannot split.
    /// </summary>
    public class SpikedSlime : EnemySlime
    {
        [SerializeField] ColliderTracker enemyUnitsTracker;
        [SerializeField] ColliderTracker slimeUnitsTracker;

        // Animation keys
        private const string ATTACK_1 = "Attack1", ATTACK_2 = "Attack2", ATTACK_3 = "Attack3";
        private const string DIE_1 = "Normal Death 1", DIE_2 = "Normal Death 2";
        private const string SPREAD_DEATH = "Spread Death";

        private int attackIndex = 0;

        [HideInInspector] public float aoeAttackRange, spreadRange;

        Dictionary<int, string> IndexAttackKeyPairs;


        protected override void Start()
        {
            enemyUnitsTracker.GetComponent<CircleCollider2D>().radius = aoeAttackRange;
            slimeUnitsTracker.GetComponent<CircleCollider2D>().radius = spreadRange;

            IndexAttackKeyPairs = new Dictionary<int, string>
        {
            { 0, ATTACK_1 },
            { 1, ATTACK_2 },
            { 2, ATTACK_3 }
        };

            canSplit = false;
            base.Start();
        }

        protected override IEnumerator MoveToTarget(Vector3 target)
        {
            if (HasCombatTarget())
            {
                Debug.Log("Has combat target");
            }

            else
            {
                StartCoroutine(base.MoveToTarget(target));
                yield break;
            }
        }

        public override void SetCombatTarget(Character pCombatTarget)
        {
            StopAllCoroutines();
            base.SetCombatTarget(pCombatTarget);
        }

        public override void ExitAttackState()
        {
            StopAllCoroutines();
            base.ExitAttackState();

            JumpCoroutine = Jump();
            StartCoroutine(JumpCoroutine);
        }

        /// <summary>
        /// Override the default death state to turn surrounding slimes into spiked slimes
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator EnterDeathState()
        {
            bool hasSurroundingSlime = false;

            foreach (Character character in enemyUnitsTracker.Colliders)
            {
                if (character != null && character is EnemySlime)
                {
                    hasSurroundingSlime = true;
                }
            }

            if (hasSurroundingSlime)
            {
                SetAnimationState(SPREAD_DEATH);
            }
            else
            {
                int random = Random.Range(0, 2);

                if (random == 0)
                {
                    SetAnimationState(DIE_1);
                }
                else
                {
                    SetAnimationState(DIE_2);
                }
            }

            velocity = Vector3.zero;

            PlayDeathSound();

            healthBar.Hide();

            eventBus.Publish("EnemyKilled", this);

            State = CharacterState.Dead;

            combatTarget = null;

            yield return new WaitForSeconds(deathAnimationLength);

            if (hasSurroundingSlime)
            {
                SetNearbySpikedSlime();
            }

            Destroy(gameObject);

            yield break;
        }

        /// <summary>
        /// Sets all nearby slimes to be spiked slimes
        /// </summary>
        private void SetNearbySpikedSlime()
        {         
            foreach (Character character in enemyUnitsTracker.Colliders)
            {
                if (character != null && character is EnemySlime && character is not SpikedSlime)
                {
                    EnemySlime enemySlime = (EnemySlime)character;
                    enemySlime.SetAsSpikedSlime();
                }
            }           
        }

        /// <summary>
        /// Override the attack coroutine to attack all militia units within the attack range
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator AttackTarget()
        {
            while (HasCombatTarget() && !IsDead())
            {
                // mover.agent.acceleration = 100;

                if (IsDead()) // Check if the character is dead and exit the coroutine if true
                {
                    yield break;
                }

                // Move towards the combat target
                if (Vector3.Distance(transform.position, combatTarget.transform.position) > attackRange)
                {
                    //  mover.SetTarget(combatTarget.transform);
                    yield return null; // Wait for the next frame to check the distance again
                }

                else
                {
                    State = CharacterState.Attacking;

                    DoSpikeSlimeAttackAnimation();

                    yield return new WaitForSeconds(attackHitMark);

                    PlayAttackSound();

                    foreach (Character character in enemyUnitsTracker.Colliders)
                    {
                        if (character != null && character is MilitiaUnit)
                        {
                            character.IntakeDamage(damage);
                        }
                    }

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
        /// Play an attack animation and cycle to the next one
        /// </summary>
        protected void DoSpikeSlimeAttackAnimation()
        {
            SetAnimationState(IndexAttackKeyPairs[attackIndex]);

            attackIndex = (attackIndex + 1) % 3;
        }

        protected override void PlayJumpSound()
        {
            audioManager.PlayOneShot(fmodEvents.spikedSlimeJumpSound, transform.position);
        }

        protected override void PlayLandSound()
        {
            audioManager.PlayOneShot(fmodEvents.spikedSlimeLandSound, transform.position);
        }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.spikedSlimeDeathSound, transform.position);
        }
    }
}