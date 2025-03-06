    using System.Collections.Generic;
using UnityEngine;
using Towers;
using Enemies;
using Core;

namespace GameManagement
{
    /// <summary>
    /// Applies the settings given from the GameSettings scriptable object to all the given game's elements
    /// </summary>
    /// 
    public class SettingsApplier : Singleton<SettingsApplier>
    {
        [Header("Tower Scripts")]
        [SerializeField] private ArcherTower archerTower;
        [SerializeField] private CatapultTower catapultTower;
        [SerializeField] private MageTower mageTower;
        [SerializeField] private MilitiaTower militiaTower;

        [Header("Enemy Scripts")]
        [SerializeField] private EnemyOrc orc;
        [SerializeField] private WolfEnemy wolf;
        [SerializeField] private EnemySlime slime;
        [SerializeField] private SpikedSlime spikedSlime;
        [SerializeField] private MountedOrc mountedOrc;
        [SerializeField] private FlyingEnemy bee;
        [SerializeField] private QueenBee queenBee;
        [SerializeField] private BeeHive beeHive;
        [SerializeField] private EnemySquid squid;
        [SerializeField] private EnemyAngler angler;
        [SerializeField] private TurtleEnemy turtle;
        [SerializeField] private FlyingEnemy gull;
        //[SerializeField] private Enemy kingAngler;
        [SerializeField] private EnemyGiantSquid giantSquid;
        [SerializeField] private ElderTurtle elderTurtle;
        [SerializeField] private EnemyLarva larva;
        [SerializeField] private EnemyWitch witch;
     //   [SerializeField] private Enemy Lizard;
        [SerializeField] private BombBatEnemy bombBat;
     //   [SerializeField] private Enemy giantLizard;
      //  [SerializeField] private Enemy queenLarva;
      //  [SerializeField] private Enemy treeman;

        [SerializeField] private PurchaseManager purchaseManager;

        [SerializeField] private GameSettings defaultGameSettings;

        protected override void Awake()
        {
            base.Awake();

            if (defaultGameSettings != null)
            {
                ApplySettings(defaultGameSettings);
            }
        }

        public void ApplySettings(GameSettings settings)
        {
            #region Towers
            // Archer Tower
            archerTower.StartingDamage = settings.archerTowerSettings.StartingDamage;
            archerTower.DamageIncreasePerUpgrade = settings.archerTowerSettings.DamageIncreasePerUpgrade;

            archerTower.StartingAttackSpeed = settings.archerTowerSettings.StartingAttackSpeed;
            archerTower.AttackSpeedIncreasePerUpgrade = settings.archerTowerSettings.AttackSpeedIncreasePerUpgrade;

            archerTower.StartingRange = settings.archerTowerSettings.StartingRange;
            archerTower.RangeIncreasePerUpgrade = settings.archerTowerSettings.RangeIncreasePerUpgrade;

            purchaseManager.SetPurchaseCost(TowerType.Archer, settings.archerTowerSettings.PurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.Archer, settings.archerTowerSettings.TotalUpgradeCost);

            // Catapult Tower
            catapultTower.StartingDamage = settings.catapultTowerSettings.StartingDamage;
            catapultTower.DamageIncreasePerUpgrade = settings.catapultTowerSettings.DamageIncreasePerUpgrade;

            catapultTower.StartingAttackSpeed = settings.catapultTowerSettings.StartingAttackSpeed;
            catapultTower.AttackSpeedIncreasePerUpgrade = settings.catapultTowerSettings.AttackSpeedIncreasePerUpgrade;

            catapultTower.StartingRange = settings.catapultTowerSettings.StartingRange;
            catapultTower.RangeIncreasePerUpgrade = settings.catapultTowerSettings.RangeIncreasePerUpgrade;

            purchaseManager.SetPurchaseCost(TowerType.Bomber, settings.catapultTowerSettings.PurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.Bomber, settings.catapultTowerSettings.TotalUpgradeCost);

            catapultTower.ProjectileChanges = new List<ProjectileChange>(settings.catapultTowerSettings.ProjectileChanges);

            // Mage Tower
            mageTower.StartingDamage = settings.mageTowerSettings.StartingDamage;
            mageTower.DamageIncreasePerUpgrade = settings.mageTowerSettings.DamageIncreasePerUpgrade;

            mageTower.StartingAttackSpeed = settings.mageTowerSettings.StartingAttackSpeed;
            mageTower.AttackSpeedIncreasePerUpgrade = settings.mageTowerSettings.AttackSpeedIncreasePerUpgrade;

            mageTower.StartingRange = settings.mageTowerSettings.StartingRange;
            mageTower.RangeIncreasePerUpgrade = settings.mageTowerSettings.RangeIncreasePerUpgrade;

            purchaseManager.SetPurchaseCost(TowerType.Mage, settings.mageTowerSettings.PurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.Mage, settings.mageTowerSettings.TotalUpgradeCost);

            // Milita Tower
            militiaTower.StartingUnitDamage = settings.militiaTowerSettings.UnitStartingDamage;
            militiaTower.UnitDamageIncreasePerUpgrade = settings.militiaTowerSettings.UnitDamageIncreasePerUpgrade;

            militiaTower.StartingUnitHealth = settings.militiaTowerSettings.UnitStartingHealth;
            militiaTower.UnitHealthIncreasePerUpgrade = settings.militiaTowerSettings.UnitHealthIncreasePerUpgrade;

            militiaTower.UnitAttackSpeed = settings.militiaTowerSettings.UnitAttackSpeed;

            militiaTower.UnitPercentHealPerSecond = settings.militiaTowerSettings.UnitPercentHealPerSecond;

            militiaTower.unitRespawnTime = settings.militiaTowerSettings.UnitRespawnTime;

            militiaTower.StartingRange = settings.militiaTowerSettings.PlacementRange;

            purchaseManager.SetPurchaseCost(TowerType.MenAtArms, settings.militiaTowerSettings.PurchaseCost);
            purchaseManager.SetTotalUpgradeCost(TowerType.MenAtArms, settings.militiaTowerSettings.TotalUpgradeCost);
            #endregion

            #region Enemies

            // Orc
            orc.maxHealth = settings.orcSettings.Health;
            orc.damage = settings.orcSettings.Damage;
            orc.movementSpeed = settings.orcSettings.Speed;
            orc.carriedMoney = settings.orcSettings.MoneyCarried;
            orc.attackSpeed = settings.orcSettings.AttackSpeed;
            orc.attackRange = settings.orcSettings.AttackRange;

            // wolf
            wolf.maxHealth = settings.wolfSettings.Health;
            wolf.damage = settings.wolfSettings.Damage;
            wolf.movementSpeed = settings.wolfSettings.Speed;
            wolf.carriedMoney = settings.wolfSettings.MoneyCarried;
            wolf.attackSpeed = settings.wolfSettings.AttackSpeed;
            wolf.attackRange = settings.wolfSettings.AttackRange;

            // Slime
            slime.maxHealth = settings.slimeSettings.Health;
            slime.damage = settings.slimeSettings.Damage;
            slime.splitChance = settings.slimeSettings.SplitChance;
            slime.carriedMoney = settings.slimeSettings.MoneyCarried;
            slime.miniSpikedSlimeDamage = settings.spikedSlimeSettings.MiniSpikedSlimeDamage;
            slime.miniSpikedSlimeHealth = settings.spikedSlimeSettings.MiniSpikedSlimeHealth;

            // Spiked Slime
            spikedSlime.maxHealth = settings.spikedSlimeSettings.Health;
            spikedSlime.damage = settings.spikedSlimeSettings.Damage;
            spikedSlime.carriedMoney = settings.spikedSlimeSettings.MoneyCarried;
            spikedSlime.attackSpeed = settings.spikedSlimeSettings.AttackSpeed;
            spikedSlime.attackRange = settings.spikedSlimeSettings.AttackRange; 
            spikedSlime.aoeAttackRange = settings.spikedSlimeSettings.AttackRange;
            spikedSlime.spreadRange = settings.spikedSlimeSettings.SpreadRange;

            // Mounted Orc

            mountedOrc.maxHealth = settings.mountedOrcSettings.Health;
            mountedOrc.damage = settings.mountedOrcSettings.Damage;
            mountedOrc.movementSpeed = settings.mountedOrcSettings.Speed;
            mountedOrc.chargeSpeed = settings.mountedOrcSettings.ChargeSpeed;
            mountedOrc.chargeDamage = settings.mountedOrcSettings.ChargeDamage;
            mountedOrc.attackSpeed = settings.mountedOrcSettings.AttackSpeed;
            mountedOrc.carriedMoney = settings.mountedOrcSettings.MoneyCarried;
            mountedOrc.timeBeforeCharge = settings.mountedOrcSettings.TimeBeforeCharge;
            mountedOrc.attackRange = settings.mountedOrcSettings.AttackRange;   

            // Bee
            bee.maxHealth = settings.beeSettings.Health;
            bee.movementSpeed = settings.beeSettings.Speed;
            bee.carriedMoney = settings.beeSettings.MoneyCarried;

            // Queen Bee 
            queenBee.maxHealth = settings.queenBeeSettings.Health;
            queenBee.damage = settings.queenBeeSettings.Damage;
            queenBee.carriedMoney = settings.queenBeeSettings.MoneyCarried;
            queenBee.movementSpeed = settings.queenBeeSettings.Speed;
            queenBee.attackSpeed = settings.queenBeeSettings.AttackSpeed;
            queenBee.minDistanceToSpawn = settings.queenBeeSettings.HiveMinEndDistance;
            queenBee.spawnInterval = settings.queenBeeSettings.HiveSpawnInterval;
            queenBee.attackRange = settings.queenBeeSettings.AttackRange;

            // Bee Hive
            beeHive.maxHealth = settings.beeHiveSettings.Health;
            beeHive.carriedMoney = settings.beeHiveSettings.MoneyCarried;
            beeHive.spawnTime = settings.beeHiveSettings.SpawnTime;
            beeHive.deathSpawnQuantity = settings.beeHiveSettings.DeathSpawnQuantity;
            beeHive.maxSpawnQuantity = settings.beeHiveSettings.MaxSpawnQuantity;

            // Squid
            squid.maxHealth = settings.squidSettings.Health;
            squid.movementSpeed = settings.squidSettings.Speed;
            squid.carriedMoney = settings.squidSettings.MoneyCarried;
            //squid.moveInterval = settings.squidMoveInterval;
            //squid.travelDuration = settings.squidTravelDuration;
            //squid.travelDistance = settings.squidTravelDistance;

            // Angler
            angler.maxHealth = settings.anglerSettings.Health;
            angler.movementSpeed = settings.anglerSettings.Speed;
            angler.damage = settings.anglerSettings.Damage;
            angler.attackSpeed = settings.anglerSettings.AttackSpeed;
            angler.carriedMoney = settings.anglerSettings.MoneyCarried;
            angler.attackRange = settings.anglerSettings.AttackRange;

            // Turtle
            turtle.maxHealth = settings.turtleSettings.Health;
            turtle.movementSpeed = settings.turtleSettings.Speed;
            turtle.damage = settings.turtleSettings.Damage;
            turtle.attackSpeed = settings.turtleSettings.AttackSpeed;
            turtle.carriedMoney = settings.turtleSettings.MoneyCarried;
            turtle.attackRange = settings.turtleSettings.AttackRange;

           // Elder Turtle
            elderTurtle.maxHealth = settings.elderTurtleSettings.Health;
            elderTurtle.movementSpeed = settings.elderTurtleSettings.Speed;
            elderTurtle.damage = settings.elderTurtleSettings.Damage;
            elderTurtle.attackSpeed = settings.elderTurtleSettings.AttackSpeed;
            elderTurtle.carriedMoney = settings.elderTurtleSettings.MoneyCarried;
            elderTurtle.specialStateDuration = settings.elderTurtleSettings.SpecialStateDuration;
            elderTurtle.intakeDPSThreshold = settings.elderTurtleSettings.ShellAbilityDPSThreshold;
            elderTurtle.specialAbilityCooldown = settings.elderTurtleSettings.ShellAbilityCooldown;
            elderTurtle.attackRange = settings.elderTurtleSettings.AttackRange;

            // Gull
            gull.maxHealth = settings.seagullSettings.Health;
            gull.movementSpeed = settings.seagullSettings.Speed;
            gull.moneyCarried = settings.seagullSettings.MoneyCarried;

            // King Angler
            //kingAngler.maxHealth = settings.kingAnglerSettings.Health;
            //kingAngler.movementSpeed = settings.kingAnglerSettings.Speed;
            //kingAngler.damage = settings.kingAnglerSettings.Damage;
            //kingAngler.attackSpeed = settings.kingAnglerAttackSettings.Speed;
            //kingAngler.carriedMoney = settings.kingAnglerCarriedMoney;

            // Giant Squid
            giantSquid.maxHealth = settings.giantSquidSettings.Health;
            giantSquid.movementSpeed = settings.giantSquidSettings.Speed;
            giantSquid.carriedMoney = settings.giantSquidSettings.MoneyCarried;
            giantSquid.DashSpeed = settings.giantSquidSettings.DashSpeed;
            //giantSquid.moveInterval = settings.giantSquidMoveInterval;
            //giantSquid.travelDuration = settings.giantSquidTravelDuration;
            //giantSquid.travelDistance = settings.giantSquidTravelDistance;



            // Larva
            larva.maxHealth = settings.larvaSettings.Health;
            larva.movementSpeed = settings.larvaSettings.Speed;
            larva.damage = settings.larvaSettings.Damage;
            larva.attackSpeed = settings.larvaSettings.AttackSpeed;
            larva.carriedMoney = settings.larvaSettings.MoneyCarried;
            larva.attackRange = settings.larvaSettings.AttackRange;

            // Witch
            witch.maxHealth = settings.witchSettings.Health;
            witch.movementSpeed = settings.witchSettings.Speed;
            witch.damage = settings.witchSettings.Damage;
            witch.rangedAttackRange = settings.witchSettings.AttackRange;
            witch.attackSpeed = settings.witchSettings.AttackSpeed;
            witch.carriedMoney = settings.witchSettings.MoneyCarried;

            // Lizard
            //Lizard.maxHealth = settings.lizardSettings.Health;
            //Lizard.movementSpeed = settings.lizardSettings.Speed;
            //Lizard.damage = settings.lizardSettings.Damage;
            //Lizard.attackSpeed = settings.lizardAttackSettings.Speed;
            //Lizard.carriedMoney = settings.lizardSettings.MoneyCarried;

            // Bomb Bat
            bombBat.maxHealth = settings.bombBatSettings.Health;
            bombBat.movementSpeed = settings.bombBatSettings.Speed;
            bombBat.damage = settings.bombBatSettings.Damage;
            bombBat.carriedMoney = settings.bombBatSettings.MoneyCarried;
            bombBat.explosionRadius = settings.bombBatSettings.ExplosionRadius;

            //// Giant Lizard
            //giantLizard.maxHealth = settings.giantLizardSettings.Health;
            //giantLizard.movementSpeed = settings.giantLizardSettings.Speed;
            //giantLizard.damage = settings.giantLizardSettings.Damage;
            //giantLizard.attackSpeed = settings.giantLizardAttackSettings.Speed;
            //giantLizard.carriedMoney = settings.giantLizardSettings.MoneyCarried;

            //// Queen Larva
            //queenLarva.maxHealth = settings.queenLarvaSettings.Health;
            //queenLarva.movementSpeed = settings.queenLarvaSettings.Speed;
            //queenLarva.damage = settings.queenLarvaSettings.Damage;
            //queenLarva.attackSpeed = settings.queenLarvaAttackSettings.Speed;
            //queenLarva.carriedMoney = settings.queenLarvaSettings.MoneyCarried;

            //// Treeman
            //treeman.maxHealth = settings.treemanSettings.Health;
            //treeman.movementSpeed = settings.treemanSettings.Speed;
            //treeman.damage = settings.treemanSettings.Damage;
            //treeman.attackSpeed = settings.treemanAttackSettings.Speed;
            //treeman.carriedMoney = settings.treemanSettings.MoneyCarried;

            #endregion
        }
    }
}
