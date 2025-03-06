using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Core.Character;

namespace Militia
{
    /// <summary>
    /// A unit that is created by the Militia Tower. Derives from the Character class and works with the Rally Point system.
    /// </summary>
    public class MilitiaUnit : Character
    {
        private Transform positionMark;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private const float fadeSpeed = 0.3f;
        public float percentHealPerSecond = 5;
        [Header("The unit must not receive damage for this much time before it can heal")]
        [SerializeField] private float healTime = 3;
        private float healTimer = 0;

        [SerializeField] private bool killUnit = false;

        [SerializeField] private float positionMarkStopDistance = 0.15f;

        public MilitiaUnitEvent UnitDeathEvent;

        // Used only by the militia unit as it is the only character that enters an idle state
        protected Dictionary<ViewDirection, string> IdleAnimationKeyPairs;

        // Additional animation keys
        private const string

            // Not yet implemented
            BLOCK_UP = "BlockRight",
            BLOCK_DOWN = "BlockDown",
            BLOCK_LEFT = "BlockLeft",
            BLOCK_RIGHT = "BlockRight",

            IDLE_UP = "IdleUp",
            IDLE_DOWN = "IdleDown",
            IDLE_LEFT = "IdleLeft",
            IDLE_RIGHT = "IdleRight";


        protected override void Start()
        {
            base.Start();

            movementTargetPos = positionMark.position;

            StartCoroutine(DoIdle());
            StartCoroutine(DoPassiveHeal());
        }

        protected override void Update()
        {
            base.Update();

            // check if the health is less than 0, if so, kill the unit
            if (killUnit)
            {
                UnitDeathEvent.Invoke(this);
                KillUnit();

                killUnit = false;
            }
        }

        protected override void InstantiateDictionaries()
        {
            // Call the base class method to instantiate the base dictionaries
            base.InstantiateDictionaries();

            IdleAnimationKeyPairs = new Dictionary<ViewDirection, string>
        {
            { ViewDirection.Up, IDLE_UP },
            { ViewDirection.Down, IDLE_DOWN },
            { ViewDirection.Left, IDLE_LEFT },
            { ViewDirection.Right, IDLE_RIGHT }
        };
        }

        protected override void MoveToTargetPosition()
        {
            if (IsDead() || movementTargetPos == Vector3.zero || State == CharacterState.Attacking)
            {
                velocity = Vector3.zero;
                return;
            }

            if (movementTargetPos == positionMark.position && Vector3.Distance(transform.position, movementTargetPos) < positionMarkStopDistance)
            {
                velocity = Vector3.zero;
                return;
            }

            // Move towards the waypoint
            Vector3 direction = (movementTargetPos - transform.position).normalized;

            // Velocity based movement
            velocity += movementSpeed * Time.deltaTime * direction;

            // Apply damping to simulate sliding
            velocity *= movementDamping;

            transform.position += velocity;
        }

        public void ShowUnit()
        {
            StartCoroutine(FadeInRenderer());
        }

        public void HideUnit()
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }

        public void ReloadHealthBar()
        {
            healthBar.InitializeHealthBar(maxHealth, CurrentHealth);
        }

        public void ResetUnit()
        {
            StopAllCoroutines();

            if (!gameObject.activeInHierarchy)
                return;

            StartCoroutine(DoIdle());
            StartCoroutine(DoPassiveHeal());

            CurrentHealth = maxHealth;

            healthBar.Show();

            ReloadHealthBar();

            combatTarget = null;

            visualTransform.GetComponent<SpriteRenderer>().flipX = false;

            attacking = false;

            DeathCoroutine = null;

            // Resets the target to the position mark
            movementTargetPos = positionMark.position;

            State = CharacterState.Normal;
        }

        public void KillUnit()
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(EnterDeathState());
        }

        public void SetPositionMark(Transform mark)
        {
            positionMark = mark;

            movementTargetPos = positionMark.position;
        }

        public override bool HasCombatTarget()
        {
            // Check to see if the combat target is not null and not dead
            if (combatTarget != null && !combatTarget.IsDead())
                return true;

            // In any other case, (target null or target is dead) return false
            // assistingCombat = false;
            attacking = false;
            return false;
        }

        private bool CanHeal()
        {
            return healTimer >= healTime;
        }

        public override void ExitAttackState()
        {
            movementTargetPos = positionMark.position;
            base.ExitAttackState();
        }

        // Increases the alpha of the spriteRenderer over time
        private IEnumerator FadeInRenderer()
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

            while (spriteRenderer.color.a < 1)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + fadeSpeed);
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        /// <summary>
        /// Override the base class method to disable and not destroy the game object, as it will respawn
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator EnterDeathState()
        {
            healthBar.Hide();

            State = CharacterState.Dead;

            combatTarget = null;

            SetAnimationState(DeathAnimationKeyPairs[viewDirection]);

            yield return new WaitForSeconds(deathAnimationLength);

            spriteRenderer.flipX = false;

            gameObject.SetActive(false);

            yield break;
        }

        /// <summary>
        /// Continously check if the unit is not attacking and heal the unit by a certain percentage of its max health per second
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoPassiveHeal()
        {
            while (true)
            {
                healTimer += 1;

                if (CanHeal())
                {
                    if (CurrentHealth < maxHealth)
                    {
                        CurrentHealth += (maxHealth * (percentHealPerSecond / 100));
                        healthBar.SetHealth(CurrentHealth);
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// Continously check if the unit is not moving and change the view direction of the unit to a random direction
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoIdle()
        {
            float random = 0;

            while (true)
            {
                random = UnityEngine.Random.Range(4, 5);
                yield return new WaitForSeconds(random);

                if (velocity == Vector3.zero && State != CharacterState.Attacking)
                {
                    int randomDirection = UnityEngine.Random.Range(0, 2);

                    if (randomDirection == 0)
                    {
                        viewDirection = ViewDirection.Left;
                    }
                    else
                    {
                        viewDirection = ViewDirection.Right;
                    }
                }
            }
        }

        public override void IntakeDamage(float amount)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            healTimer = 0;

            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                if (DeathCoroutine == null)
                {
                    // Invoke the event to notify the tower that the unit has died
                    UnitDeathEvent.Invoke(this);

                    DeathCoroutine = EnterDeathState();
                    StartCoroutine(DeathCoroutine);
                }
            }

            if (healthBar != null)
                healthBar.SetHealth(CurrentHealth);
        }

        /// <summary>
        /// Override the base class method to include the idle animations
        /// </summary>
        protected override void DoWalkAnimation()
        {
            if (State == CharacterState.Dead || State == CharacterState.Attacking)
            {
                return;
            }

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
                    SetAnimationState(WALK_UP);
                }
                else if (angle >= 135 || angle < -135)
                {
                    viewDirection = ViewDirection.Left;
                    SetAnimationState(WALK_LEFT);
                }
                else if (angle >= -135 && angle < -45)
                {
                    viewDirection = ViewDirection.Down;
                    SetAnimationState(WALK_DOWN);
                }
                else if (angle >= -45 && angle < 45)
                {
                    viewDirection = ViewDirection.Right;
                    SetAnimationState(WALK_RIGHT);
                }
            }

            // If the character is not moving, animate an idle animation corresponding to the current view direction
            else
            {
                SetAnimationState(IdleAnimationKeyPairs[viewDirection]);
            }
        }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.humanDeathSound, transform.position);
        }

        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.humanAttackSound, transform.position);
        }
    }

    [Serializable]
    public class MilitiaUnitEvent : UnityEvent<MilitiaUnit> { }
}

