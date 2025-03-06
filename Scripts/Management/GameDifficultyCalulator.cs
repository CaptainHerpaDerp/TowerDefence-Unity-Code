using Core;
using Enemies;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement
{
    public class GameDifficultyCalculator : Singleton<GameDifficultyCalculator>
    {
        private Dictionary<EnemyType, float> enemyDifficultyRatings = new();

        public Dictionary<EnemyType, float> EnemyDifficultyRatings => enemyDifficultyRatings;

        [Header("Vale Weights")]
        [SerializeField] private float healthWeight;
        [SerializeField] private float damageWeight;
        [SerializeField] private float attackSpeedWeight;
        [SerializeField] private float speedWeight;

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
                { EnemyType.Orc, gameSettings.orcSettings.Health },
                { EnemyType.Wolf, gameSettings.wolfSettings.Health },
                { EnemyType.Slime, gameSettings.slimeSettings.Health },
                { EnemyType.MountedOrc, gameSettings.mountedOrcSettings.Health },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeSettings.Health },
                { EnemyType.Bee, gameSettings.beeSettings.Health },
                { EnemyType.QueenBee, gameSettings.queenBeeSettings.Health },
                { EnemyType.Squid, gameSettings.squidSettings.Health },
                { EnemyType.Angler, gameSettings.anglerSettings.Health },
                { EnemyType.Turtle, gameSettings.turtleSettings.Health },
                { EnemyType.Gull, gameSettings.seagullSettings.Health },
                { EnemyType.KingAngler, gameSettings.kingAnglerSettings.Health },
                { EnemyType.GiantSquid, gameSettings.giantSquidSettings.Health},
                { EnemyType.ElderTurtle, gameSettings.elderTurtleSettings.Health },
                { EnemyType.Larva, gameSettings.larvaSettings.Health },
                { EnemyType.Witch, gameSettings.witchSettings.Health },
                { EnemyType.Lizard, gameSettings.lizardSettings.Health },
                { EnemyType.BombBat, gameSettings.bombBatSettings.Health },
                { EnemyType.GiantLizard, gameSettings.giantLizardSettings.Health },
                { EnemyType.QueenLarva, gameSettings.queenLarvaSettings.Health },
                { EnemyType.Treeman, gameSettings.treemanSettings.Health }
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
                { EnemyType.Orc, gameSettings.orcSettings.Damage },
                { EnemyType.Wolf, gameSettings.wolfSettings.Damage },
                { EnemyType.Slime, gameSettings.slimeSettings.Damage },
                { EnemyType.MountedOrc, gameSettings.mountedOrcSettings.Damage },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeSettings.Damage },
                { EnemyType.QueenBee, gameSettings.queenBeeSettings.Damage },
                { EnemyType.Angler, gameSettings.anglerSettings.Damage },
                { EnemyType.Turtle, gameSettings.turtleSettings.Damage },
                { EnemyType.KingAngler, gameSettings.kingAnglerSettings.Damage },
                { EnemyType.ElderTurtle, gameSettings.elderTurtleSettings.Damage },
                { EnemyType.Larva, gameSettings.larvaSettings.Damage },
                { EnemyType.Witch, gameSettings.witchSettings.Damage },
                { EnemyType.Lizard, gameSettings.lizardSettings.Damage },
                { EnemyType.BombBat, gameSettings.bombBatSettings.Damage },
                { EnemyType.GiantLizard, gameSettings.giantLizardSettings.Damage },
                { EnemyType.QueenLarva, gameSettings.queenLarvaSettings.Damage },
                { EnemyType.Treeman, gameSettings.treemanSettings.Damage}
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
                { EnemyType.Orc, gameSettings.orcSettings.Speed },
                { EnemyType.Wolf, gameSettings.wolfSettings.Speed },
                // Slime has no Settings.Speed, set it to the same as wolf (relatively similar Settings.Speed)
                { EnemyType.Slime, gameSettings.wolfSettings.Speed },
                { EnemyType.MountedOrc, gameSettings.mountedOrcSettings.Speed },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeSettings.Speed },
                { EnemyType.Bee, gameSettings.beeSettings.Speed },
                { EnemyType.Squid, gameSettings.squidSettings.Speed },
                { EnemyType.Gull, gameSettings.seagullSettings.Speed },
                { EnemyType.GiantSquid, gameSettings.giantSquidSettings.Speed },
                { EnemyType.QueenBee, gameSettings.queenBeeSettings.Speed },
                { EnemyType.Angler, gameSettings.anglerSettings.Speed },
                { EnemyType.Turtle, gameSettings.turtleSettings.Speed },
                { EnemyType.KingAngler, gameSettings.kingAnglerSettings.Speed },
                { EnemyType.ElderTurtle, gameSettings.elderTurtleSettings.Speed },
                { EnemyType.Larva, gameSettings.larvaSettings.Speed },
                { EnemyType.Witch, gameSettings.witchSettings.Speed },
                { EnemyType.Lizard, gameSettings.lizardSettings.Speed },
                { EnemyType.BombBat, gameSettings.bombBatSettings.Speed },
                { EnemyType.GiantLizard, gameSettings.giantLizardSettings.Speed },
                { EnemyType.QueenLarva, gameSettings.queenLarvaSettings.Speed },
                { EnemyType.Treeman, gameSettings.treemanSettings.Speed }
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
                { EnemyType.Orc, gameSettings.orcSettings.AttackSpeed },
                { EnemyType.Wolf, gameSettings.wolfSettings.AttackSpeed },
                { EnemyType.Slime, 1 },
                { EnemyType.MountedOrc, gameSettings.mountedOrcSettings.AttackSpeed },
                { EnemyType.SpikedSlime, gameSettings.spikedSlimeSettings.AttackSpeed },
                { EnemyType.QueenBee, gameSettings.queenBeeSettings.AttackSpeed },
                { EnemyType.Angler, gameSettings.anglerSettings.AttackSpeed },
                { EnemyType.Turtle, gameSettings.turtleSettings.AttackSpeed },
                { EnemyType.KingAngler, gameSettings.kingAnglerSettings.AttackSpeed },
                { EnemyType.ElderTurtle, gameSettings.elderTurtleSettings.AttackSpeed },
                { EnemyType.Larva, gameSettings.larvaSettings.AttackSpeed },
                { EnemyType.Witch, gameSettings.witchSettings.AttackSpeed },
                { EnemyType.Lizard, gameSettings.lizardSettings.AttackSpeed },
                { EnemyType.GiantLizard, gameSettings.giantLizardSettings.AttackSpeed },
                { EnemyType.QueenLarva, gameSettings.queenLarvaSettings.AttackSpeed },
                { EnemyType.Treeman, gameSettings.treemanSettings.AttackSpeed }
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