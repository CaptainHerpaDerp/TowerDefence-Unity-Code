using Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public class GameDifficultyCalulator : MonoBehaviour
    {
        public static GameDifficultyCalulator Instance;

        private Dictionary<EnemyType, float> enemyDifficultyRatings = new();

        public Dictionary<EnemyType, float> EnemyDifficultyRatings => enemyDifficultyRatings;

        [Header("Vale Weights")]
        [SerializeField] private float healthWeight;
        [SerializeField] private float damageWeight;
        [SerializeField] private float attackSpeedWeight;
        [SerializeField] private float speedWeight;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("GameDifficultyCalulator already exists. Deleting duplicate.");
                Destroy(gameObject);
            }
        }

        public void DoDifficultyRatingCalculations(GameSettings gameSettings)
        {
            enemyDifficultyRatings = new Dictionary<EnemyType, float>() {
            { EnemyType.Orc, 0 },
            { EnemyType.Wolf, 0 },
            { EnemyType.Slime, 0 },
            { EnemyType.MountedOrc, 0 },
            { EnemyType.SpikedSlime, 0 },
            { EnemyType.Bee, 0 },
            { EnemyType.Squid, 0 },
            { EnemyType.Gull, 0 },
            { EnemyType.GiantSquid, 0 },
            { EnemyType.QueenBee, 0 },
            { EnemyType.Angler, 0 },
            { EnemyType.Turtle, 0 },
            { EnemyType.KingAngler, 0 },
            { EnemyType.ElderTurtle, 0 },
            { EnemyType.Larva, 0 },
            { EnemyType.Witch, 0 },
            { EnemyType.Lizard, 0 },
            { EnemyType.BombBat, 0 },
            { EnemyType.GiantLizard, 0 },
            { EnemyType.QueenLarva, 0 },
            { EnemyType.Treeman, 0 }
            };


            #region Health Calculations

            float lowestEnemyHealth = float.MaxValue;
            float highestEnemyHealth = float.MinValue;

            Dictionary<EnemyType, float> enemyHealths = new()
            {
                { EnemyType.Orc, gameSettings.orcHealth },
                { EnemyType.Wolf, gameSettings.wolfHealth },
                { EnemyType.Slime, gameSettings.slimeHealth },
                { EnemyType.MountedOrc, gameSettings.mountedOrcHealth },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeHealth },
                { EnemyType.Bee, gameSettings.beeHealth },
                { EnemyType.QueenBee, gameSettings.queenBeeHealth },
                { EnemyType.Squid, gameSettings.squidHealth },
                { EnemyType.Angler, gameSettings.anglerHealth },
                { EnemyType.Turtle, gameSettings.turtleHealth },
                { EnemyType.Gull, gameSettings.gullHealth },
                { EnemyType.KingAngler, gameSettings.kingAnglerHealth },
                { EnemyType.GiantSquid, gameSettings.giantSquidHealth},
                { EnemyType.ElderTurtle, gameSettings.elderTurtleHealth },
                { EnemyType.Larva, gameSettings.larvaHealth },
                { EnemyType.Witch, gameSettings.witchHealth },
                { EnemyType.Lizard, gameSettings.lizardHealth },
                { EnemyType.BombBat, gameSettings.bombBatHealth },
                { EnemyType.GiantLizard, gameSettings.giantLizardHealth },
                { EnemyType.QueenLarva, gameSettings.queenLarvaHealth },
                { EnemyType.Treeman, gameSettings.treemanHealth }
            };

            // Get the enemy with the lowest health
            foreach (float health in enemyHealths.Values)
            {
                if (health < lowestEnemyHealth)
                {
                    lowestEnemyHealth = health;
                }
            }

            // Get the enemy with the highest health
            foreach (float health in enemyHealths.Values)
            {
                if (health > highestEnemyHealth)
                {
                    highestEnemyHealth = health;
                }
            }

            #endregion

            #region Damage Calculations

            float lowestEnemyDamage = float.MaxValue;
            float highestEnemyDamage = float.MinValue;

            Dictionary<EnemyType, float> enemyDamages = new()
            {
                { EnemyType.Orc, gameSettings.orcDamage },
                { EnemyType.Wolf, gameSettings.wolfDamage },
                { EnemyType.Slime, gameSettings.slimeDamage },
                { EnemyType.MountedOrc, gameSettings.mountedOrcDamage },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeDamage },
                { EnemyType.QueenBee, gameSettings.queenBeeDamage },
                { EnemyType.Angler, gameSettings.anglerDamage },
                { EnemyType.Turtle, gameSettings.turtleDamage },
                { EnemyType.KingAngler, gameSettings.kingAnglerDamage },
                { EnemyType.ElderTurtle, gameSettings.elderTurtleDamage },
                { EnemyType.Larva, gameSettings.larvaDamage },
                { EnemyType.Witch, gameSettings.witchDamage },
                { EnemyType.Lizard, gameSettings.lizardDamage },
                { EnemyType.BombBat, gameSettings.bombBatDamage },
                { EnemyType.GiantLizard, gameSettings.giantLizardDamage },
                { EnemyType.QueenLarva, gameSettings.queenLarvaDamage },
                { EnemyType.Treeman, gameSettings.treemanDamage}
            };

            // Get the enemy with the lowest damage
            foreach (float damage in enemyDamages.Values)
            {
                if (damage < lowestEnemyDamage)
                {
                    lowestEnemyDamage = damage;
                }
            }

            // Get the enemy with the highest damage
            foreach (float damage in enemyDamages.Values)
            {
                if (damage > highestEnemyDamage)
                {
                    highestEnemyDamage = damage;
                }
            }

            #endregion

            #region Speed Calculations

            float lowestEnemySpeed = float.MaxValue;
            float highestEnemySpeed = float.MinValue;

            Dictionary<EnemyType, float> enemySpeeds = new()
            {
                { EnemyType.Orc, gameSettings.orcSpeed },
                { EnemyType.Wolf, gameSettings.wolfSpeed },
                // Slime has no speed, set it to the same as wolf (relatively similar speed)
                { EnemyType.Slime, gameSettings.wolfSpeed },
                { EnemyType.MountedOrc, gameSettings.mountedOrcSpeed },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeSpeed },
                { EnemyType.Bee, gameSettings.beeSpeed },
                { EnemyType.Squid, gameSettings.squidSpeed },
                { EnemyType.Gull, gameSettings.gullSpeed },
                { EnemyType.GiantSquid, gameSettings.giantSquidSpeed },
                { EnemyType.QueenBee, gameSettings.queenBeeSpeed },
                { EnemyType.Angler, gameSettings.anglerSpeed },
                { EnemyType.Turtle, gameSettings.turtleSpeed },
                { EnemyType.KingAngler, gameSettings.kingAnglerSpeed },
                { EnemyType.ElderTurtle, gameSettings.elderTurtleSpeed },
                { EnemyType.Larva, gameSettings.larvaSpeed },
                { EnemyType.Witch, gameSettings.witchSpeed },
                { EnemyType.Lizard, gameSettings.lizardSpeed },
                { EnemyType.BombBat, gameSettings.bombBatSpeed },
                { EnemyType.GiantLizard, gameSettings.giantLizardSpeed },
                { EnemyType.QueenLarva, gameSettings.queenLarvaSpeed },
                { EnemyType.Treeman, gameSettings.treemanSpeed }
            };

            // Get the enemy with the lowest speed
            foreach (float speed in enemySpeeds.Values)
            {
                if (speed < lowestEnemySpeed)
                {
                    lowestEnemySpeed = speed;
                }
            }

            // Get the enemy with the highest speed
            foreach (float speed in enemySpeeds.Values)
            {
                if (speed > highestEnemySpeed)
                {
                    highestEnemySpeed = speed;
                }
            }

            #endregion

            #region AttackSpeed Calculations

            // These values are a bit different, the lower the interval, the higher the attack speed
            float lowestEnemyAttackSpeed = float.MinValue;
            float highestEnemyAttackSpeed = float.MaxValue;

            Dictionary<EnemyType, float> enemyAttackSpeeds = new()
            {
                { EnemyType.Orc, gameSettings.orcAttackSpeed },
                { EnemyType.Wolf, gameSettings.wolfAttackSpeed },
                { EnemyType.Slime, 1 },
                { EnemyType.MountedOrc, gameSettings.mountedOrcAttackSpeed },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeAttackSpeed },
                { EnemyType.QueenBee, gameSettings.queenBeeAttackSpeed },
                { EnemyType.Angler, gameSettings.anglerAttackSpeed },
                { EnemyType.Turtle, gameSettings.turtleAttackSpeed },
                { EnemyType.KingAngler, gameSettings.kingAnglerAttackSpeed },
                { EnemyType.ElderTurtle, gameSettings.elderTurtleAttackSpeed },
                { EnemyType.Larva, gameSettings.larvaAttackSpeed },
                { EnemyType.Witch, gameSettings.witchAttackSpeed },
                { EnemyType.Lizard, gameSettings.lizardAttackSpeed },
                { EnemyType.GiantLizard, gameSettings.giantLizardAttackSpeed },
                { EnemyType.QueenLarva, gameSettings.queenLarvaAttackSpeed },
                { EnemyType.Treeman, gameSettings.treemanAttackSpeed }
            };

            // Get the enemy with the lowest attackspeed
            foreach (float attackspeed in enemyAttackSpeeds.Values)
            {
                if (attackspeed < highestEnemyAttackSpeed)
                {
                    highestEnemyAttackSpeed = attackspeed;
                }
            }

            // Get the enemy with the highest attackspeed
            foreach (float attackspeed in enemyAttackSpeeds.Values)
            {
                if (attackspeed > lowestEnemyAttackSpeed)
                {
                    lowestEnemyAttackSpeed = attackspeed;
                }
            }

            #endregion

            // Calculate individual difficulty ratings

            foreach (EnemyType enemyType in enemyHealths.Keys)
            {
                float healthRating = 0, damageRating = 0, speedRating = 0, attackSpeedRating = 0;

                if (enemyHealths.ContainsKey(enemyType))
                    healthRating = (enemyHealths[enemyType] - lowestEnemyHealth) / (highestEnemyHealth - lowestEnemyHealth);

                if (enemyDamages.ContainsKey(enemyType))
                    damageRating = (enemyDamages[enemyType] - lowestEnemyDamage) / (highestEnemyDamage - lowestEnemyDamage);

                if (enemySpeeds.ContainsKey(enemyType))
                    speedRating = (enemySpeeds[enemyType] - lowestEnemySpeed) / (highestEnemySpeed - lowestEnemySpeed);

                if (enemyAttackSpeeds.ContainsKey(enemyType))
                    attackSpeedRating = (enemyAttackSpeeds[enemyType] - lowestEnemyAttackSpeed) / (highestEnemyAttackSpeed - lowestEnemyAttackSpeed);

                enemyDifficultyRatings[enemyType] = ((healthRating * healthWeight) + (damageRating * damageWeight) + (speedRating * speedWeight) + (attackSpeedRating * attackSpeedWeight)) * 10;
            }
        }
    }
}