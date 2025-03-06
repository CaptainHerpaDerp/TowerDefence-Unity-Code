using AudioManagement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Character
{
    public enum CharacterState
    {
        Normal,
        Attacking,
        Dead
    }

    public enum ViewDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Class shared by all "road" characters in the game, including the militia units and enemies
    /// </summary>
    public abstract class Character : MonoBehaviour
    {
        [BoxGroup("Component References"), SerializeField] protected HealthBar healthBar;
        [BoxGroup("Component References"), SerializeField] protected Transform visualTransform;
        [BoxGroup("Component References"), SerializeField] protected Animator visualAnimator;

        [Header("Damping factor affects movement and has the unit 'slide'")]
        [BoxGroup("Movement Variables"), SerializeField] protected float movementDamping = 0.9f;

        [BoxGroup("Animation Variables"), SerializeField] protected float minAngleForVerticalAnimation = 0.03f;
        [BoxGroup("Animation Variables"), SerializeField] protected float attackHitMark;
        [BoxGroup("Animation Variables"), SerializeField] protected float runAnimationSpeed;

        // Animation Parameters

        [InfoBox("These settings are modified by the game settings applier, so there is no use in modifying them here"), FoldoutGroup("Character Settings"), ReadOnly]
        public float movementSpeed = 1f;
        [FoldoutGroup("Character Settings"), ReadOnly] public float maxHealth;
        [FoldoutGroup("Character Settings"), ReadOnly] public float damage;
        [FoldoutGroup("Character Settings"), ReadOnly] public int carriedMoney;
        [FoldoutGroup("Character Settings"), ReadOnly] public float attackRange;
        [FoldoutGroup("Character Settings"), ReadOnly] public float attackSpeed = 0.8f;

        // Movement
        protected Vector3 movementTargetPos;
        protected Vector3 velocity;
        private float preservedMovementDamping;

        // Character State
        protected ViewDirection viewDirection = ViewDirection.Down;
        public CharacterState State { get; protected set; } = CharacterState.Normal;

        // Combat
        protected bool attacking;
        protected Character combatTarget;
        protected CircleCollider2D aggressionTrigger;

        // Sound and Events Managers
        protected AudioManager audioManager;
        protected FMODEvents fmodEvents;
        protected EventBus eventBus;

        // Health
        [ReadOnly] public float CurrentHealth;
        protected const float deathAnimationLength = 0.75f;

        // Temporary Variables
        protected float savedEndPointDistance;
        protected float movingAcceleration;
        protected Vector3 preservedVelocity;
        protected string savedAnimationState;

        public bool EngagesInCombat { get; protected set; } = true;

        // Coroutine for Attack
        protected IEnumerator AttackCoroutine;

        // Coroutine for Death
        protected IEnumerator DeathCoroutine;

        // Animation keys
        protected string
            ATTACK_UP = "AttackUp",
            ATTACK_DOWN = "AttackDown",
            ATTACK_LEFT = "AttackLeft",
            ATTACK_RIGHT = "AttackRight",

            WALK_UP = "WalkUp",
            WALK_DOWN = "WalkDown",
            WALK_LEFT = "WalkLeft",
            WALK_RIGHT = "WalkRight",

            DIE_UP = "DieUp",
            DIE_DOWN = "DieDown",
            DIE_LEFT = "DieLeft",
            DIE_RIGHT = "DieRight";

        private Vector3 NewPos;
        protected Vector3 ObjVelocity;
        private Vector3 PrevPos;

        protected Dictionary<ViewDirection, string> DeathAnimationKeyPairs;

        // Time buffer for sprite direction changes
        protected const float minDirectionChangeTime = 0.5f;
        protected float lastDirectionChangeTime;

        protected virtual void Start()
        {
            // Singleton assignments
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;
            eventBus = EventBus.Instance;

            // levelEventManager = LevelEventManager.Instance;

            CurrentHealth = maxHealth;

            if (healthBar != null)
            {
                healthBar.InitializeHealthBar(maxHealth, CurrentHealth);
            }
            else
            {
                Debug.LogError("Health bar is not assigned!");
            }

            visualAnimator = visualTransform.GetComponent<Animator>();

            if (visualAnimator == null && visualTransform != null)
            {
                Debug.LogError("Could not find the visualAnimator within the visual transform!");
            }
            else if (visualTransform == null)
            {
                Debug.LogError("Visual transform is not assigned!");
            }

            InstantiateDictionaries();
        }

        protected virtual void Update()
        {
            TrackMovement();
            DoWalkAnimation();
            MoveToTargetPosition();
        }

        protected virtual void InstantiateDictionaries()
        {
            DeathAnimationKeyPairs = new Dictionary<ViewDirection, string>
        {
            { ViewDirection.Up, DIE_UP },
            { ViewDirection.Down, DIE_DOWN },
            { ViewDirection.Left, DIE_LEFT },
            { ViewDirection.Right, DIE_RIGHT }
        };
        }

        #region Movement Methods

        protected void TrackMovement()
        {
            NewPos = transform.position;
            ObjVelocity = (NewPos - PrevPos) / Time.fixedDeltaTime;
            PrevPos = NewPos;
        }

        protected virtual void MoveToTargetPosition()
        {
            if (IsDead() || movementTargetPos == Vector3.zero || State == CharacterState.Attacking)
            {
                return;
            }

            // Move towards the waypoint
            Vector3 direction = (movementTargetPos - transform.position).normalized;

            // Velocity based movement
            velocity += direction * movementSpeed * Time.deltaTime;

            // Apply damping to simulate sliding
            velocity *= movementDamping;

            transform.position += velocity;
        }

        #endregion

        #region Combat Methods
        public virtual bool HasCombatTarget()
        {
            // Check to see if the combat target is not null and not dead
            if (combatTarget != null && !combatTarget.IsDead())
                return true;

            // In any other case, (target null or target is dead) return false
            return false;
        }

        public virtual Character GetCombatTarget()
        {
            return combatTarget;
        }

        public virtual void SetCombatTarget(Character pCombatTarget)
        {
            combatTarget = pCombatTarget;
            movementTargetPos = combatTarget.transform.position;

            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;
            }

            AttackCoroutine = AttackTarget();
            StartCoroutine(AttackCoroutine);
        }

        protected virtual IEnumerator AttackTarget()
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
                    State = CharacterState.Attacking;

                    // Within attack range, initiate attack
                    Vector3 direction = (combatTarget.transform.position - transform.position).normalized;
                    float angle = Vector2.SignedAngle(Vector2.right, direction);

                    DoAttackAnimation(angle);

                    yield return new WaitForSeconds(attackHitMark);

                    PlayAttackSound();

                    if (HasCombatTarget())
                        combatTarget.IntakeDamage(damage);

                    // If target is still not dead, wait for the attack movementSpeed and attack again
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

        public virtual void IntakeDamage(float amount)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                if (DeathCoroutine == null)
                {
                    DeathCoroutine = EnterDeathState();
                    StartCoroutine(DeathCoroutine);
                }
            }

            if (healthBar != null)
                healthBar.SetHealth(CurrentHealth);
        }

        /// <summary>
        /// Applies a slow effect for a certain duration
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="duration"></param>
        public virtual void ApplySlowEffect(float amount, float duration)
        {
            StartCoroutine(DoSlowEffect(amount, duration));
        }

        private IEnumerator DoSlowEffect(float amount, float duration)
        {
            float originalSpeed = movementSpeed;
            float speedReductionPercent = amount;
            float elapsedTime = 0f;

            // Apply movementSpeed reduction
            movementSpeed *= speedReductionPercent;

            // Gradually restore movementSpeed to original value
            while (elapsedTime < duration)
            {
                movementSpeed += (originalSpeed - movementSpeed) * (Time.deltaTime / duration);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure movementSpeed returns to original value
            movementSpeed = originalSpeed;
        }

        protected virtual IEnumerator EnterDeathState()
        {
            velocity = Vector3.zero;

            PlayDeathSound();

            healthBar.Hide();

            eventBus.Publish("EnemyKilled", this);

            State = CharacterState.Dead;

            combatTarget = null;

            SetAnimationState(DeathAnimationKeyPairs[viewDirection]);

            yield return new WaitForSeconds(deathAnimationLength);

            Destroy(gameObject);

            yield break;
        }

        public virtual void ExitAttackState()
        {
            combatTarget = null;
            State = CharacterState.Normal;
        }

        public bool IsDead()
        {
            return gameObject == null || !gameObject.activeInHierarchy || State == CharacterState.Dead;
        }

        #endregion

        #region Animation

        protected virtual void DoAttackAnimation(float angle)
        {
            if (angle >= 45 && angle < 135)
            {
                RepeatAnimationState(ATTACK_UP);
                viewDirection = ViewDirection.Up;
            }
            // If the target is to the left
            else if (angle >= 135 || angle < -135)
            {
                RepeatAnimationState(ATTACK_LEFT);
                viewDirection = ViewDirection.Left;
            }
            else if (angle >= -135 && angle < -45)
            {
                RepeatAnimationState(ATTACK_DOWN);
                viewDirection = ViewDirection.Down;
            }
            else if (angle >= -45 && angle < 45)
            {
                RepeatAnimationState(ATTACK_RIGHT);
                viewDirection = ViewDirection.Right;
            }
        }

        protected virtual void DoWalkAnimation()
        {
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

                if (Time.time - lastDirectionChangeTime < minDirectionChangeTime)
                {
                    return;
                }

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

                lastDirectionChangeTime = Time.time;
            }
        }

        #endregion

        #region Animation Key Methods

        protected virtual void SetAnimationState(string newState)
        {
            visualTransform.GetComponent<SpriteRenderer>().flipX = false;
            visualAnimator.Play(newState);
        }

        /// <summary>
        /// Used when the same animation needs to be repeated, eg. Catapult has one animation, 
        /// cannot set the animation state to the same animation, so "rewinds" the animation
        /// </summary>
        /// <param name="newState"></param>
        protected void RepeatAnimationState(string newState)
        {
            visualAnimator.CrossFadeInFixedTime(newState, 0.01f);
        }

        #endregion

        #region Sound Methods
        protected abstract void PlayDeathSound();

        protected abstract void PlayAttackSound();

        #endregion

        #region Event Methods
        public void OnGamePaused()
        {
            preservedVelocity = velocity;
            preservedMovementDamping = movementDamping;
            velocity = Vector3.zero;
            movementDamping = 0;
        }

        public void OnGameUnPaused()
        {
            velocity = preservedVelocity;
            movementDamping = preservedMovementDamping;
        }

        #endregion
    }

}