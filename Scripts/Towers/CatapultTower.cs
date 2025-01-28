using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    /// <summary>
    /// A class to store the number of projectiles fired at each level
    /// </summary>
    [System.Serializable]
    public class ProjectileChange
    {
        public int level;
        public int numberOfProjectiles;
    }

    /// <summary>
    /// A tower that fires a cluster of projectiles in the direction of the target
    /// </summary>
    public class CatapultTower : Tower
    {
        // A set of positions for each cluster projectile
        private HashSet<Vector3> generatedPositions = new();

        [Header("Projectile Variables")]
        public float minDistanceBetweenProjectiles = 2f; // Adjust this value as needed

        private float projectilesPerAttack;

        [HideInInspector] public List<ProjectileChange> ProjectileChanges;

        [SerializeField] private int maxIterations = 100;

        // Override base start method to set the number of projectiles per attack to the first level
        protected override void Start()
        {
            base.Start();
            projectilesPerAttack = ProjectileChanges[0].numberOfProjectiles;
            soundEffectManager.PlayCatapultTowerUpgradeSound(1);
        }

        /// <summary>
        /// Overrides the default upgrade method, sets the number of projectiles per attack based on the level
        /// </summary>
        /// <param name="level"></param>
        public override void UpgradeTower(int level)
        {
            foreach (ProjectileChange projectileChange in ProjectileChanges)
            {
                if (level == projectileChange.level)
                {
                    projectilesPerAttack = projectileChange.numberOfProjectiles;
                }
            }

            base.UpgradeTower(level);

            soundEffectManager.PlayCatapultTowerUpgradeSound(currentLevel);
        }

        /// <summary>
        /// Overrides the default attack method, fires a "cluster" of projectiles from the catapult unit
        /// </summary>
        /// <param name="unit"></param>

        protected override IEnumerator FireProjectile(TowerUnit unit)
        {
            unit.AttackTowards(currentTarget.transform);

            Vector2 targetPosition = currentTarget.transform.position;

            if (currentLevel < 6)
            {
                soundEffectManager.PlaySmallCatapultSound();
            }
            else
            {
                soundEffectManager.PlayLargeCatapultSound();
            }

            yield return new WaitForSeconds(unit.hitMarkTime);

            if (unit == null || isUpgrading)
            {
                fireCoroutine = null;
                yield break;
            }

            // Sets the projectile to face upwards when it is created
            Quaternion initialQuat = Quaternion.Euler(0, 0, -90);

            for (int i = 0; i < projectilesPerAttack; i++)
            {
                Vector3 randomPosition = FindValidRandomPosition(targetPosition, minDistanceBetweenProjectiles);
                if (randomPosition == Vector3.zero)
                {
                    // Unable to find a valid position, exit the loop
                    break;
                }

                generatedPositions.Add(randomPosition);

                ProjectileSpawnRequest spawnRequest = new ProjectileSpawnRequest
                {
                    projectileType = ProjectileType.Cluster,
                    spawnPosition = unit.transform.GetChild(0).GetChild(0).transform.position,
                    spawnRotation = initialQuat,
                    projectileDamage = AttackDamage,
                    attackTarget = null,
                    preferredPosition = randomPosition  
                };

                eventBus.Publish("ProjectileSpawnRequest", spawnRequest);
            }

            // Clear the list for the next attack
            generatedPositions.Clear();

            fireCoroutine = null;
            yield break; 
        }

        /// <summary>
        /// Returns a random position within the minimum distance of the target position and other generated positions
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="minDistance"></param>
        private Vector3 FindValidRandomPosition(Vector3 targetPosition, float minDistance)
        {
            int iterations = 0;

            while (iterations < maxIterations)
            {
                Vector3 randomPosition = GetRandomizedPosition(targetPosition);

                if (!IsTooCloseToGeneratedPositions(randomPosition, minDistance))
                {
                    return randomPosition;
                }

                iterations++;
            }

            // Unable to find a valid position
            return Vector3.zero;
        }

        /// <summary>
        /// Returns true if the given position is too close to any of the generated positions
        /// </summary>
        /// <param name="position"></param>
        /// <param name="minDistance"></param>
        private bool IsTooCloseToGeneratedPositions(Vector3 position, float minDistance)
        {
            foreach (Vector3 generatedPosition in generatedPositions)
            {
                // Use sqrMagnitude for distance comparison to avoid square root calculations
                if ((position - generatedPosition).sqrMagnitude < minDistance * minDistance)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a randomized position within the minimum distance of the given position
        /// </summary>
        /// <param name="position"></param>
        private Vector3 GetRandomizedPosition(Vector3 position)
        {
            float xOffset = Random.Range(-minDistanceBetweenProjectiles, minDistanceBetweenProjectiles);
            float yOffset = Random.Range(-minDistanceBetweenProjectiles, minDistanceBetweenProjectiles);

            return new Vector3(position.x + xOffset, position.y + yOffset, position.z);
        }
    }
}
