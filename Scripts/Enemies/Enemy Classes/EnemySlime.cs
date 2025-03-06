using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Core.Character;
using Sirenix.OdinInspector;

namespace Enemies
{
    public class EnemySlime : Enemy
    {
        [BoxGroup("Jump Settings"), SerializeField] protected float jumpDistance, minDistance, airborneTime, jumpIntervalTime = 1, jumpPointRandomness, splitDistance;

        [BoxGroup("Library Assets"), SerializeField] private SpriteLibraryAsset smallSlimeSprite, spikedSlimeSprite;

        protected bool splitting, hasSplit, canSplit = true;

        // Set to true when a spiked slime converts this slime
        protected bool isSpikedSlime;

        public int splitChance;

        // Values for determining the point to play the jump sound
        private const float landtime = 0.2f;
        private const float jumpTime = 0.2f;
        private const float SplitAnimationDuration = 0.6f;

        [HideInInspector] public float miniSpikedSlimeDamage, miniSpikedSlimeHealth;

        protected IEnumerator JumpCoroutine;

        private const string SPLIT = "SlimeSplit";

        [BoxGroup("Debugging"), SerializeField] private bool debugSplit, jumpAtStart = true;

        protected override void Start()
        {
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
                Debug.LogError("Could not find the animator within the visual transform!");
            }
            else if (visualTransform == null)
            {
                Debug.LogError("Visual transform is not assigned!");
            }

            base.Start();

            if (jumpAtStart)
            {
                JumpCoroutine = Jump();
                StartCoroutine(JumpCoroutine);
            }
        }

        protected override void Update()
        {   
            if (debugSplit)
            {
                debugSplit = false;
                Split();
            }
        }

        protected IEnumerator Jump()
        {
            while (true)
            {
                if (IsDead() || splitting)
                {
                    yield break;
                }

                CalculateNewBouncePoint();

                yield return new WaitForSeconds(jumpIntervalTime);
            }
        }

        private void DoSplitChance()
        {
            if (IsOverWater()|| IsDead() || splitting || hasSplit || !canSplit)
            {
                return;
            }

            if (Random.Range(0, splitChance + 1) == 0)
            {
                Split();
            }
        }

        /// <summary>
        /// Sets the slime as a spiked slime, changing its sprite and increasing its health and damage
        /// </summary>
        public void SetAsSpikedSlime()
        {
            if (hasSplit)
                return;

            isSpikedSlime = true;
            canSplit = false;
            visualTransform.GetComponent<SpriteLibrary>().spriteLibraryAsset = spikedSlimeSprite;

            // Change the slime's variables and re-initialize the health bar
            damage = miniSpikedSlimeDamage;
            maxHealth = miniSpikedSlimeHealth;
            CurrentHealth = maxHealth;
            healthBar.InitializeHealthBar(maxHealth, CurrentHealth);
        }

        private void Split()
        {
            splitting = true;
            StartCoroutine(DoSplit());
        }

        /// <summary>
        /// Splits the current slime into two smaller slimes after a short animation delay
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoSplit()
        {
            SetAnimationState(SPLIT);

            // Wait for the animation to finish
            yield return new WaitForSeconds(SplitAnimationDuration);

            if (IsDead())
            {
                yield break;
            }

            // Instantiate a copy of this slime
            GameObject splitSlime = Instantiate(gameObject, transform.position, Quaternion.identity);

            // Move the new slime to the left and the current slime to the right
            splitSlime.transform.position += new Vector3(-splitDistance, 0, 0);
            transform.position += new Vector3(splitDistance, 0, 0);

            EnemySlime newEnemySlime = splitSlime.GetComponent<EnemySlime>();
            newEnemySlime.Start();

            InstantiateEnemy(newEnemySlime);

            // get the current health percentage
            float healthPercentage = CurrentHealth / maxHealth;

            splitSlime.transform.SetParent(transform.parent);

            yield return new WaitForFixedUpdate();
            newEnemySlime.SetAsSplit(healthPercentage);

            // Sets the current slime as split
            SetAsSplit(healthPercentage);

            if (JumpCoroutine != null)
            {
                StopCoroutine(JumpCoroutine);
                yield return new WaitForSeconds(0.2f); // Small delay before restarting movement
                JumpCoroutine = Jump();
                StartCoroutine(JumpCoroutine);
            }


            yield break;
        }

        /// <summary>
        /// Gets the position of the next waypoint, randomizes it and jumps towards it
        /// </summary>
        private void CalculateNewBouncePoint()
        {
            if (splitting) return; // Prevents a jump from being queued if splitting is imminent

            if (roadWaypoints == null)
            {
                Debug.LogError("Road waypoints are not assigned!");
            }

            if (roadWaypoints.Count == 0)
            {
                Debug.LogError("No road waypoints are assigned!");
                return;
            }

            Vector3 nextWaypointPosition = roadWaypoints[CurrentWaypointIndex];

            // Randomize the next point
            nextWaypointPosition = RandomizeCornerPosition(nextWaypointPosition);

            JumpTowardsPosition(nextWaypointPosition);
        }

        /// <summary>
        /// Returns a modified position within the jumpPointRandomness range
        /// </summary>
        /// <param name="cornerPosition"></param>
        private Vector3 RandomizeCornerPosition(Vector3 cornerPosition)
        {
            return cornerPosition + new Vector3(Random.Range(-jumpPointRandomness, jumpPointRandomness), Random.Range(-jumpPointRandomness, jumpPointRandomness), 0);
        }

        /// <summary>
        /// Moves the enemy towards the specified position
        /// </summary>
        /// <param name="movementTarget"></param>
        private void JumpTowardsPosition(Vector3 movementTarget)
        {
            // Calculate the direction to the target
            Vector3 direction = (movementTarget - transform.position).normalized;

            PlayJumpingAnimation(direction);

            StartCoroutine(MoveToTarget(direction * jumpDistance));
        }

        /// <summary>
        /// Jump towards the target position
        /// </summary>
        /// <param name="target"></param>
        protected virtual IEnumerator MoveToTarget(Vector3 target)
        {
            // Get the initial position of the object
            Vector3 initialPosition = transform.position;

            float elapsedTime = 0f;

            bool playedJumpSound = false;
            bool playedLandSound = false;

            while (elapsedTime < airborneTime)
            {
                if (IsDead())
                {
                    yield break;
                }

                // Increment the elapsed time based on the movement speed
                elapsedTime += Time.deltaTime;

                // Calculate the interpolation factor using EaseInOutCubic function
                float t = EaseInOutCubic(elapsedTime / airborneTime);

                // Use the eased value to interpolate between the initial and target positions
                transform.position = Vector3.Lerp(initialPosition, initialPosition + target, t);

                if (elapsedTime > jumpTime && !playedJumpSound)
                {
                    PlayJumpSound();
                    playedJumpSound = true;
                }

                if (elapsedTime > airborneTime - landtime && !playedLandSound)
                {
                    PlayLandSound();
                    playedLandSound = true;
                }

                // Wait for the next frame
                yield return null;
            }

            // Ensure that the object is exactly at the target position after the loop
            transform.position = initialPosition + target;

            DoSplitChance();

            yield break;
        }

        /// <summary>
        /// Overrides the base attack target method to allow the slime to jump towards the target and deal damage when it is close enough
        /// </summary>
        /// <returns></returns>
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
                    State = CharacterState.Attacking;

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

        private float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }

        private void PlayJumpingAnimation(Vector3 direction)
        {
            if (splitting)
                return;

            float angle = Vector2.SignedAngle(Vector2.right, new Vector2(direction.x, direction.y));

            if (angle >= 45 && angle < 135)
            {
                // Jumping Up
                RepeatAnimationState(WALK_UP);
            }
            else if (angle >= 135 || angle < -135)
            {
                // Jumping Left
                RepeatAnimationState(WALK_LEFT);
            }
            else if (angle >= -135 && angle < -45)
            {
                // Jumping Down
                RepeatAnimationState(WALK_DOWN);
            }
            else if (angle >= -45 && angle < 45)
            {
                // Jumping Right
                RepeatAnimationState(WALK_RIGHT);
            }
        }

        /// <summary>
        /// Marks this slime as split so that it doesn't split again. Also halves the health and changes the sprite to a smaller version
        /// </summary>
        public void SetAsSplit(float healthPercentage)
        {
            splitting = false;
            hasSplit = true;
            visualTransform.GetComponent<SpriteLibrary>().spriteLibraryAsset = smallSlimeSprite;
            maxHealth /= 2;
            carriedMoney /= 2;
            CurrentHealth = maxHealth * healthPercentage;
            healthBar.InitializeHealthBar(maxHealth, CurrentHealth);
        }

        #region Audio Methods

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.slimeDeathSound, transform.position);   
        }
            
        protected virtual void PlayJumpSound()
        {

        }

        protected virtual void PlayLandSound()
        {

        }

        // The slime does not have an attack
        protected override void PlayAttackSound() { }

        #endregion
    }
}