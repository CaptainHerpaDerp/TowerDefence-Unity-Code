using System.Collections;
using UnityEngine;
using Core;
using Core.Character;

namespace Enemies
{
    /// <summary>
    /// An enemy that spawns bees at a constant rate until it reaches its max spawn quantity and destroys itself
    /// </summary>
    public class BeeHive : Enemy
    {
        private const string DEATH = "Beehive Death";

        [HideInInspector] public float spawnTime;
        [HideInInspector] public int deathSpawnQuantity;
        [HideInInspector] public int maxSpawnQuantity;

        // Pre set end distance set to the queen bee's current end distance at the time of spawning
        [HideInInspector] public float endDistance;

        private int spawnQuantity;

        [SerializeField] Enemy beePrefab;

        protected override void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;
            eventBus = EventBus.Instance;

            CurrentHealth = maxHealth;

            if (healthBar != null)
            {
                healthBar.InitializeHealthBar(maxHealth, CurrentHealth);
            }
            else
            {
                Debug.LogError("Health bar is not assigned!");
            }

            if (!visualTransform.TryGetComponent(out animator))
            {
                Debug.LogError("Could not find the animator within the visual transform!");
            }

            else if (visualTransform == null)
            {
                Debug.LogError("Visual transform is not assigned!");
            }

            soundEffectManager.PlayBeehiveRaiseSound();

            StartCoroutine(SpawnUnit());
        }

        /// <summary>
        /// Overrides the base class method to spawn any remaining bees and destroy itself
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator EnterDeathState()
        {
            PlayDeathSound();

            healthBar.Hide();

            eventBus.Publish("EnemyKilled", this);

            State = CharacterState.Dead;

            SetAnimationState(DEATH);

            // Spawn remaining bees, up to the max death spawn quantity
            int totalDeathSpawn = maxSpawnQuantity - spawnQuantity;
            if (totalDeathSpawn > deathSpawnQuantity)
            {
                totalDeathSpawn = deathSpawnQuantity;
            }

            if (totalDeathSpawn > 0)
            {
                for (int i = 0; i < totalDeathSpawn; i++)
                {
                    Enemy newBee = Instantiate(beePrefab, parent: transform.parent);
                    newBee.transform.position = transform.position + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0);

                    InstantiateEnemy(newBee);

                    // Sets the new bee's carried money to 0 to avoid exploitation.
                    newBee.moneyCarried = 0;
                }
            }

            yield return new WaitForSeconds(deathAnimationLength);

            Destroy(gameObject);

            yield break;
        }

        /// <summary>
        /// Constantly spawn bees from the hive's position until the max spawn quantity is reached
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnUnit()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnTime);

                Enemy newBee = Instantiate(beePrefab, parent: transform.parent);
                newBee.transform.position = transform.position;

                InstantiateEnemy(newBee);

                // Sets the new bee's carried money to 0 to avoid exploitation.
                newBee.moneyCarried = 0;

                spawnQuantity++;
                if (spawnQuantity >= maxSpawnQuantity)
                {
                    StartCoroutine(EnterDeathState());
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Overrides the base class method to return the end distance of the hive which is set to the queen bee's current end distance at the time of spawning
        /// </summary>
        public override float GetTotalDistanceToTravel()
        {
            return endDistance;
        }

        protected override void PlayDeathSound()
        {
            soundEffectManager.PlayBeehiveDeathSound();
        }
    }

}