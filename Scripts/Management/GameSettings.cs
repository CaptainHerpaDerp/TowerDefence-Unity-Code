using System.Collections.Generic;
using UnityEngine;
using Towers;

[System.Serializable]
public class GameSettings : ScriptableObject
{
    // Global Settings
    public int towerSellReturnPercentage = 40;

    #region Tower Settings

    // Archer Tower
    public int archerTowerTotalUpgradeCost;
    public int archerTowerPurchaseCost;

    public float archerTowerStartingDamage;
    public float archerTowerDamageIncreasePerUpgrade;

    public float archerTowerStartingAttackSpeed ;
    public float archerTowerAttackSpeedIncreasePerUpgrade;

    public float archerTowerStartingRange;
    public float archerTowerRangeIncreasePerUpgrade;

    // Mage Tower
    public int mageTowerTotalUpgradeCost;
    public int mageTowerPurchaseCost;

    public float mageTowerStartingDamage;
    public float mageTowerDamageIncreasePerUpgrade;

    public float mageTowerStartingAttackSpeed;
    public float mageTowerAttackSpeedIncreasePerUpgrade;

    public float mageTowerStartingRange;
    public float mageTowerRangeIncreasePerUpgrade;

    // Catapult Tower
    public int catapultTowerTotalUpgradeCost;
    public int catapultTowerPurchaseCost;

    public float catapultTowerStartingDamagePerProjectile;
    public float catapultTowerProjectileDamageIncreasePerUpgrade;

    public float catapultTowerStartingAttackSpeed   ;
    public float catapultTowerAttackSpeedIncreasePerUpgrade;

    public float catapultTowerStartingRange; 
    public float catapultTowerRangeIncreasePerUpgrade;

    [SerializeReference]
    public List<ProjectileChange> catapultProjectileChanges;
     
    // Militia Tower
    public int militiaTowerTotalUpgradeCost;
    public int militiaTowerPurchaseCost;

    public float militiaTowerUnitStartingDamage;
    public float militiaTowerUnitDamageIncreasePerUpgrade;

    public float militiaTowerUnitAttackSpeed;

    public float militiaTowerUnitStartingHealth;
    public float militiaTowerUnitHealthIncreasePerUpgrade;

    public float militiaUnitPercentHealPerSecond;

    public float militiaTowerPlacementRange;

    public int militiaUnitRespawnTime;

    #endregion

    #region Enemy Settings

    // Orc 
    public int orcHealth = 25;
    public float orcDamage = 5;
    public float orcSpeed = 0.2f;
    public float orcAttackSpeed = 0.8f;
    public int orcMoneyCarried;

    // Wolf
    public int wolfHealth = 10;
    public float wolfDamage = 2;
    public float wolfSpeed = 0.8f;
    public float wolfAttackSpeed = 0.8f;
    public int wolfMoneyCarried;

    // Slime
    public int slimeHealth = 10;
    public float slimeDamage = 2;
    public int slimeSplitChange = 35;
    public int slimeMoneyCarried;

     
    // Mounted Orc
    public int mountedOrcHealth = 70;
    public float mountedOrcDamage = 10;
    public float mountedOrcSpeed;
    public float mountedOrcChargeSpeed;
    public float mountedOrcChargeDamage;
    public float mountedOrcAttackSpeed = 0.8f;
    public float mountedOrcTimeBeforeCharge;
    public int mountedOrcMoneyCarried;

    // Spiked Slime
    public int spikedSlimeHealth = 20;
    public float spikedSlimeDamage = 5;
    public float spikedSlimeSpeed = 0.2f;
    public float spikedSlimeAttackSpeed = 0.8f;
    public int spikedSlimeMoneyCarried;

    public float spikedSlimeAttackRange = 0.4f;
    public float spikedSlimeSpreadRange = 0.75f;

    public float miniSpikedSlimeDamage = 4;
    public float miniSpikedSlimeHealth = 25;

    // Bee
    public int beeHealth = 10;
    public float beeSpeed = 0.2f;
    public int beeMoneyCarried;

    // Queen Bee
    public int queenBeeHealth = 10;
    public float queenBeeDamage = 2;
    public float queenBeeSpeed = 0.2f;
    public float queenBeeAttackSpeed = 0.8f;
    public int queenBeeMoneyCarried;
    public float hiveSpawnInterval;
    public float hiveMinEndDistance;
        
    // Hive
    public int beeHiveHealth = 10;
    public float beeHiveSpawnTime;
    public int beeHiveDeathSpawnQuantity;
    public int beeHiveMoneyCarried;
    public int beeHiveMaxSpawnQuantity;

    // Squid
    public int squidHealth = 10;
    public float squidSpeed;
    public int squidMoneyCarried;

    // Angler
    public int anglerHealth = 10;
    public float anglerDamage = 2;
    public float anglerSpeed = 0.2f;
    public float anglerAttackSpeed = 0.8f;
    public int anglerMoneyCarried;

    // Turtle
    public int turtleHealth = 0;
    public float turtleDamage = 0;
    public float turtleSpeed = 0;
    public float turtleAttackSpeed = 0;
    public int turtleMoneyCarried;

    // Seagull
    public int gullHealth = 10;
    public float gullSpeed = 0.2f;
    public int gullMoneyCarried;

    // King Anger
    public int kingAnglerHealth = 10;
    public float kingAnglerDamage = 2;
    public float kingAnglerSpeed = 0.2f;
    public float kingAnglerAttackSpeed = 0.8f;
    public int kingAnglerCarriedMoney;

    // Giant Squid
    public int giantSquidHealth = 10;
    public float giantSquidSpeed;
    public int giantSquidMoneyCarried;

    // Elder Turtle
    public int elderTurtleHealth = 0;
    public float elderTurtleDamage = 0;
    public float elderTurtleSpeed = 0;
    public float elderTurtleAttackSpeed = 0;
    public float specialStateDuration;
    public float shellAbilityDPSThreshold;
    public float shellAbilityCooldown;
    public int elderTurtleMoneyCarried;

    // Larva
    public int larvaHealth = 0;
    public float larvaDamage = 0;
    public float larvaSpeed = 0;
    public float larvaAttackSpeed = 0;
    public int larvaMoneyCarried;
    public float spawnTime;

    // Witch
    public int witchHealth = 0;
    public float witchDamage = 0;
    public float witchSpeed = 0;
    public float witchAttackSpeed = 0;
    public int witchMoneyCarried;
    public float witchAttackRange;

    // Lizard
    public int lizardHealth = 0;
    public float lizardDamage = 0;
    public float lizardSpeed = 0;
    public float lizardAttackSpeed = 0;
    public int lizardMoneyCarried;

    // Bomb Bat
    public int bombBatHealth = 0;
    public float bombBatDamage = 0;
    public float bombBatExplosionRadius = 0;
    public float bombBatSpeed = 0;
    public int bombBatMoneyCarried;

    // Giant Lizard
    public int giantLizardHealth = 0;
    public float giantLizardDamage = 0;
    public float giantLizardSpeed = 0;
    public float giantLizardAttackSpeed = 0;
    public int giantLizardMoneyCarried;

    // Queen Larva
    public int queenLarvaHealth = 0;
    public float queenLarvaDamage = 0;
    public float queenLarvaSpeed = 0;
    public float queenLarvaAttackSpeed = 0;
    public int queenLarvaMoneyCarried;

    // Treeman
    public int treemanHealth = 0;
    public float treemanDamage = 0;
    public float treemanSpeed = 0;
    public float treemanAttackSpeed = 0;
    public int treemanMoneyCarried;


    #endregion

}

