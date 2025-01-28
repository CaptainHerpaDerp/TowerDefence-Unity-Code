using System.Collections.Generic;
using UnityEngine;
using Towers;
using Enemies;

namespace Management
{
    /// <summary>
    /// Applies the settings given from the GameSettings scriptable object to all the given game's elements
    /// </summary>
    /// 
    public class SettingsApplier : MonoBehaviour
    {
        // Singleton Instance
        public static SettingsApplier Instance;

        [Header("Tower Scripts")]
        [SerializeField] private ArcherTower archerTower;
        [SerializeField] private CatapultTower catapultTower;
        [SerializeField] private MageTower mageTower;
        [SerializeField] private MilitiaTower militiaTower;

        [Header("Enemy Scripts")]
        [SerializeField] private Enemy orc;
        [SerializeField] private Enemy wolf;
        [SerializeField] private EnemySlime slime;
        [SerializeField] private SpikedSlime spikedSlime;
        [SerializeField] private MountedOrc mountedOrc;
        [SerializeField] private FlyingEnemy bee;
        [SerializeField] private QueenBee queenBee;
        [SerializeField] private BeeHive beeHive;
        [SerializeField] private EnemySquid squid;
        [SerializeField] private EnemyAngler angler;
        [SerializeField] private Enemy turtle;
        [SerializeField] private FlyingEnemy gull;
        [SerializeField] private Enemy kingAngler;
        [SerializeField] private EnemySquid giantSquid;
        [SerializeField] private ElderTurtle elderTurtle;
        [SerializeField] private EnemyLarva larva;
        [SerializeField] private EnemyWitch witch;
        [SerializeField] private Enemy Lizard;
        [SerializeField] private BombBatEnemy bombBat;
        [SerializeField] private Enemy giantLizard;
        [SerializeField] private Enemy queenLarva;
        [SerializeField] private Enemy treeman;

        [SerializeField] private PurchaseManager purchaseManager;

        [SerializeField] private GameSettings defaultGameSettings;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("There are multiple SettingsApplier instances in the scene. Destroying the new one.");
                Destroy(gameObject);
            }

            if (defaultGameSettings != null)
            {
                ApplySettings(defaultGameSettings);
            }
        }

        public void ApplySettings(GameSettings settings)
        {
            #region Towers
            // Archer Tower
            archerTower.StartingDamage = settings.archerTowerStartingDamage;
            archerTower.DamageIncreasePerUpgrade = settings.archerTowerDamageIncreasePerUpgrade;

            archerTower.StartingAttackSpeed = settings.archerTowerStartingAttackSpeed;
            archerTower.AttackSpeedIncreasePerUpgrade = settings.archerTowerAttackSpeedIncreasePerUpgrade;

            archerTower.StartingRange = settings.archerTowerStartingRange;
            archerTower.RangeIncreasePerUpgrade = settings.archerTowerRangeIncreasePerUpgrade;

            purchaseManager.SetPurchaseCost(TowerType.Archer, settings.archerTowerPurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.Archer, settings.archerTowerTotalUpgradeCost);

            // Catapult Tower
            catapultTower.StartingDamage = settings.catapultTowerStartingDamagePerProjectile;
            catapultTower.DamageIncreasePerUpgrade = settings.catapultTowerProjectileDamageIncreasePerUpgrade;

            catapultTower.StartingAttackSpeed = settings.catapultTowerStartingAttackSpeed;
            catapultTower.AttackSpeedIncreasePerUpgrade = settings.catapultTowerAttackSpeedIncreasePerUpgrade;

            catapultTower.StartingRange = settings.catapultTowerStartingRange;
            catapultTower.RangeIncreasePerUpgrade = settings.catapultTowerRangeIncreasePerUpgrade;

            purchaseManager.SetPurchaseCost(TowerType.Bomber, settings.catapultTowerPurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.Bomber, settings.catapultTowerTotalUpgradeCost);

            catapultTower.ProjectileChanges = new List<ProjectileChange>(settings.catapultProjectileChanges);

            // Mage Tower
            mageTower.StartingDamage = settings.mageTowerStartingDamage;
            mageTower.DamageIncreasePerUpgrade = settings.mageTowerDamageIncreasePerUpgrade;

            mageTower.StartingAttackSpeed = settings.mageTowerStartingAttackSpeed;
            mageTower.AttackSpeedIncreasePerUpgrade = settings.mageTowerAttackSpeedIncreasePerUpgrade;

            mageTower.StartingRange = settings.mageTowerStartingRange;
            mageTower.RangeIncreasePerUpgrade = settings.mageTowerRangeIncreasePerUpgrade;

            purchaseManager.SetPurchaseCost(TowerType.Mage, settings.mageTowerPurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.Mage, settings.mageTowerTotalUpgradeCost);

            // Milita Tower
            militiaTower.startingUnitDamage = settings.militiaTowerUnitStartingDamage;
            militiaTower.unitDamageIncreasePerUpgrade = settings.militiaTowerUnitDamageIncreasePerUpgrade;

            militiaTower.startingUnitHealth = settings.militiaTowerUnitStartingHealth;
            militiaTower.unitHealthIncreasePerUpgrade = settings.militiaTowerUnitHealthIncreasePerUpgrade;

            militiaTower.unitAttackSpeed = settings.militiaTowerUnitAttackSpeed;

            militiaTower.unitPercentHealPerSecond = settings.militiaUnitPercentHealPerSecond;

            militiaTower.unitRespawnTime = settings.militiaUnitRespawnTime;

            militiaTower.StartingRange = settings.militiaTowerPlacementRange;

            purchaseManager.SetPurchaseCost(TowerType.MenAtArms, settings.militiaTowerPurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.MenAtArms, settings.militiaTowerTotalUpgradeCost);
            #endregion

            #region Enemies

            // Orc
            orc.maxHealth = settings.orcHealth;
            orc.damage = settings.orcDamage;
            orc.speed = settings.orcSpeed;
            orc.attackSpeed = settings.orcAttackSpeed;
            orc.carriedMoney = settings.orcMoneyCarried;

            // wolf
            wolf.maxHealth = settings.wolfHealth;
            wolf.damage = settings.wolfDamage;
            wolf.speed = settings.wolfSpeed;
            wolf.attackSpeed = settings.wolfAttackSpeed;
            wolf.carriedMoney = settings.wolfMoneyCarried;

            // Slime
            slime.maxHealth = settings.slimeHealth;
            slime.damage = settings.slimeDamage;
            slime.splitChance = settings.slimeSplitChange;
            slime.carriedMoney = settings.slimeMoneyCarried;
            slime.miniSpikedSlimeDamage = settings.miniSpikedSlimeDamage;
            slime.miniSpikedSlimeHealth = settings.miniSpikedSlimeHealth;

            spikedSlime.maxHealth = settings.spikedSlimeHealth;
            spikedSlime.damage = settings.spikedSlimeDamage;
            spikedSlime.carriedMoney = settings.spikedSlimeMoneyCarried;
            spikedSlime.attackSpeed = settings.spikedSlimeAttackSpeed;
            spikedSlime.aoeAttackRange = settings.spikedSlimeAttackRange;
            spikedSlime.spreadRange = settings.spikedSlimeSpreadRange;

            // Mounted Orc
            mountedOrc.damage = settings.mountedOrcDamage;
            mountedOrc.speed = settings.mountedOrcSpeed;
            mountedOrc.chargeSpeed = settings.mountedOrcChargeSpeed;
            mountedOrc.chargeDamage = settings.mountedOrcChargeDamage;
            mountedOrc.attackSpeed = settings.mountedOrcAttackSpeed;
            mountedOrc.carriedMoney = settings.mountedOrcMoneyCarried;
            mountedOrc.timeBeforeCharge = settings.mountedOrcTimeBeforeCharge;

            // Bee
            bee.maxHealth = settings.beeHealth;
            bee.speed = settings.beeSpeed;
            bee.carriedMoney = settings.beeMoneyCarried;

            // Queen Bee 
            queenBee.maxHealth = settings.queenBeeHealth;
            queenBee.damage = settings.queenBeeDamage;
            queenBee.carriedMoney = settings.queenBeeMoneyCarried;
            queenBee.speed = settings.queenBeeSpeed;
            queenBee.attackSpeed = settings.queenBeeAttackSpeed;
            queenBee.minDistanceToSpawn = settings.hiveMinEndDistance;
            queenBee.spawnInterval = settings.hiveSpawnInterval;

            // Bee Hive
            beeHive.maxHealth = settings.beeHiveHealth;
            beeHive.carriedMoney = settings.beeHiveMoneyCarried;
            beeHive.spawnTime = settings.beeHiveSpawnTime;
            beeHive.deathSpawnQuantity = settings.beeHiveDeathSpawnQuantity;
            beeHive.maxSpawnQuantity = settings.beeHiveMaxSpawnQuantity;

            // Squid
            squid.maxHealth = settings.squidHealth;
            squid.speed = settings.squidSpeed;
            squid.carriedMoney = settings.squidMoneyCarried;
            //squid.moveInterval = settings.squidMoveInterval;
            //squid.travelDuration = settings.squidTravelDuration;
            //squid.travelDistance = settings.squidTravelDistance;

            // Angler
            angler.maxHealth = settings.anglerHealth;
            angler.speed = settings.anglerSpeed;
            angler.damage = settings.anglerDamage;
            angler.attackSpeed = settings.anglerAttackSpeed;
            angler.carriedMoney = settings.anglerMoneyCarried;

            // Turtle
            turtle.maxHealth = settings.turtleHealth;
            turtle.speed = settings.turtleSpeed;
            turtle.damage = settings.turtleDamage;
            turtle.attackSpeed = settings.turtleAttackSpeed;
            turtle.carriedMoney = settings.turtleMoneyCarried;

            // Gull
            gull.maxHealth = settings.gullHealth;
            gull.speed = settings.gullSpeed;
            gull.moneyCarried = settings.gullMoneyCarried;

            // King Angler
            kingAngler.maxHealth = settings.kingAnglerHealth;
            kingAngler.speed = settings.kingAnglerSpeed;
            kingAngler.damage = settings.kingAnglerDamage;
            kingAngler.attackSpeed = settings.kingAnglerAttackSpeed;
            kingAngler.carriedMoney = settings.kingAnglerCarriedMoney;

            // Giant Squid
            giantSquid.maxHealth = settings.giantSquidHealth;
            giantSquid.speed = settings.giantSquidSpeed;
            giantSquid.carriedMoney = settings.giantSquidMoneyCarried;
            //giantSquid.moveInterval = settings.giantSquidMoveInterval;
            //giantSquid.travelDuration = settings.giantSquidTravelDuration;
            //giantSquid.travelDistance = settings.giantSquidTravelDistance;

            // Elder Turtle
            elderTurtle.maxHealth = settings.elderTurtleHealth;
            elderTurtle.speed = settings.elderTurtleSpeed;
            elderTurtle.damage = settings.elderTurtleDamage;
            elderTurtle.attackSpeed = settings.elderTurtleAttackSpeed;
            elderTurtle.carriedMoney = settings.elderTurtleMoneyCarried;
            elderTurtle.specialStateDuration = settings.specialStateDuration;
            elderTurtle.intakeDPSThreshold = settings.shellAbilityDPSThreshold;
            elderTurtle.specialAbilityCooldown = settings.shellAbilityCooldown;

            // Larva
            larva.maxHealth = settings.larvaHealth;
            larva.speed = settings.larvaSpeed;
            larva.damage = settings.larvaDamage;
            larva.attackSpeed = settings.larvaAttackSpeed;
            larva.carriedMoney = settings.larvaMoneyCarried;

            // Witch
            witch.maxHealth = settings.witchHealth;
            witch.speed = settings.witchSpeed;
            witch.damage = settings.witchDamage;
            witch.rangedAttackRange = settings.witchAttackRange;
            witch.attackSpeed = settings.witchAttackSpeed;
            witch.carriedMoney = settings.witchMoneyCarried;

            // Lizard
            Lizard.maxHealth = settings.lizardHealth;
            Lizard.speed = settings.lizardSpeed;
            Lizard.damage = settings.lizardDamage;
            Lizard.attackSpeed = settings.lizardAttackSpeed;
            Lizard.carriedMoney = settings.lizardMoneyCarried;

            // Bomb Bat
            bombBat.maxHealth = settings.bombBatHealth;
            bombBat.speed = settings.bombBatSpeed;
            bombBat.damage = settings.bombBatDamage;
            bombBat.carriedMoney = settings.bombBatMoneyCarried;
            bombBat.explosionRadius = settings.bombBatExplosionRadius;

            //// Giant Lizard
            //giantLizard.maxHealth = settings.giantLizardHealth;
            //giantLizard.speed = settings.giantLizardSpeed;
            //giantLizard.damage = settings.giantLizardDamage;
            //giantLizard.attackSpeed = settings.giantLizardAttackSpeed;
            //giantLizard.carriedMoney = settings.giantLizardMoneyCarried;

            //// Queen Larva
            //queenLarva.maxHealth = settings.queenLarvaHealth;
            //queenLarva.speed = settings.queenLarvaSpeed;
            //queenLarva.damage = settings.queenLarvaDamage;
            //queenLarva.attackSpeed = settings.queenLarvaAttackSpeed;
            //queenLarva.carriedMoney = settings.queenLarvaMoneyCarried;

            //// Treeman
            //treeman.maxHealth = settings.treemanHealth;
            //treeman.speed = settings.treemanSpeed;
            //treeman.damage = settings.treemanDamage;
            //treeman.attackSpeed = settings.treemanAttackSpeed;
            //treeman.carriedMoney = settings.treemanMoneyCarried;

            #endregion
        }
    }
}
