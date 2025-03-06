using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// An enemy that can spawn beehives at a set interval at its current position.
    /// </summary>
    public class QueenBee : Enemy
    {
        [SerializeField] private BeeHive beeHivePrefab;

        // The minimum distance this unit can be from the end where it can spawn a beehive (prevents new hive too close to end)
        public float minDistanceToSpawn;

        public float spawnInterval;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(SpawnHives());
        }

        /// <summary>
        /// Continuously spawns beehives at a set interval unless the distance to the end is less than the minimum distance to spawn.
        /// </summary>
        private IEnumerator SpawnHives()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);

                // Do not spawn a new hive if the unit is not on the ground or if the unit is attacking
                if (!IsOverWater() && State != Core.Character.CharacterState.Attacking)
                {
                    // If the current distance to the end point is less than the required distance to spawn a new hive
                    if (GetTotalDistanceToTravel() > minDistanceToSpawn)
                    {
                        BeeHive beeHive = Instantiate(beeHivePrefab, parent: transform.parent);

                        beeHive.transform.position = transform.position;

                        InstantiateEnemy(beeHive);
                    
                        // Sets 
                        beeHive.endDistance = GetTotalDistanceToTravel();

                        yield return new WaitForFixedUpdate();
                    }

                    // Otherwise, break the loop
                    else
                    {
                        yield break;
                    }
                }
            }
        }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.queenBeeDeathSound, transform.position);
        }

        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.queenBeeAttackSound, transform.position);
        }
    }
}