using Core.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enemies
{
    /// <summary>
    /// An enemy that will become invulnerable and wont move for a short duration after a certain intake DPS threshold is reached
    /// </summary>
    public class ElderTurtle : Enemy
    {
        [SerializeField] private float measureTime = 1f;
        [SerializeField] private float intakeDPS;

        [SerializeField] private float specialExitDuration = 0.5f;

        public float specialStateDuration = 5f;
        public float intakeDPSThreshold;

        // The time before the special ability can be activated again
        public float specialAbilityCooldown;
        private float specialAbilityCooldownTimer;

        private bool inSpecialState;
        private float baseSpeed;

        [SerializeField] private bool forceEnterSpecialState;

        private const string
            ENTER_SHELL_RIGHT = "EnterShellRight", EXIT_SHELL_RIGHT = "ExitShellRight",
            ENTER_SHELL_LEFT = "EnterShellLeft", EXIT_SHELL_LEFT = "ExitShellLeft",
            ENTER_SHELL_UP = "EnterShellUp", EXIT_SHELL_UP = "ExitShellUp",
            ENTER_SHELL_DOWN = "EnterShellDown", EXIT_SHELL_DOWN = "ExitShellDown";


        protected override void Start()
        {
            base.Start();
            StartCoroutine(CheckDPS());
        }

        protected override void Update()
        {
            if (State != CharacterState.Dead)
            {
                // If the intake DPS is greater than the threshold, activate the special ability
                if (intakeDPS >= intakeDPSThreshold && CanEnterSpecialState() || forceEnterSpecialState)
                {
                    forceEnterSpecialState = false;
                    DoSpecialAbility();
                }

                if (specialAbilityCooldownTimer < specialAbilityCooldown)
                {
                    specialAbilityCooldownTimer += Time.deltaTime;
                }
            }

            base.Update();
        }

        private bool CanEnterSpecialState()
        {
            return specialAbilityCooldownTimer >= specialAbilityCooldown;
        }

        /// <summary>
        /// Resets the intake DPS after a certain amount of time
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckDPS()
        {
            while (true)
            {
                yield return new WaitForSeconds(measureTime);
                intakeDPS = 0;
            }
        }

        private void DoSpecialAbility()
        {
            inSpecialState = true;

            baseSpeed = speed;
            speed = 0;

            specialAbilityCooldownTimer = 0;

            switch (viewDirection)
            {
                case ViewDirection.Right:
                    SetAnimationState(ENTER_SHELL_RIGHT);
                    break;
                case ViewDirection.Left:
                    SetAnimationState(ENTER_SHELL_LEFT);
                    break;
                case ViewDirection.Up:
                    SetAnimationState(ENTER_SHELL_UP);
                    break;
                case ViewDirection.Down:
                    SetAnimationState(ENTER_SHELL_DOWN);
                    break;
            }

            StartCoroutine(ExitSpecialAbility());
        }

        private IEnumerator ExitSpecialAbility()
        {
            yield return new WaitForSeconds(specialStateDuration);

            switch (viewDirection)
            {
                case ViewDirection.Right:
                    SetAnimationState(EXIT_SHELL_RIGHT);
                    break;
                case ViewDirection.Left:
                    SetAnimationState(EXIT_SHELL_LEFT);
                    break;
                case ViewDirection.Up:
                    SetAnimationState(EXIT_SHELL_UP);
                    break;
                case ViewDirection.Down:
                    SetAnimationState(EXIT_SHELL_DOWN);
                    break;
            }

            yield return new WaitForSeconds(specialExitDuration);

            speed = baseSpeed;

            inSpecialState = false;
        }

        #region Overridden Methods

        protected override IEnumerator AttackTarget()
        {
            while (HasCombatTarget() && !IsDead())
            {
                if (inSpecialState)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

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

        protected override void DoWalkAnimation()
        {
            if (inSpecialState)
            {
                return;
            }

            base.DoWalkAnimation();
        }

        public override void IntakeDamage(float amount)
        {
            // If the character is in the special state, do not intake damage
            if (inSpecialState)
            {
                return;
            }

            base.IntakeDamage(amount);

            intakeDPS += amount;
        }

        protected override void PlayDeathSound()
        {
            base.PlayDeathSound();
        }

        #endregion
    }
}