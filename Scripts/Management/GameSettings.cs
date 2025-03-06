using System.Collections.Generic;
using UnityEngine;
using Towers;
using Sirenix.OdinInspector;

#region Tower Classes

[System.Serializable]
public abstract class RangedTowerSettings
{
    [HorizontalGroup("Row1", Width = 200)]
    [LabelWidth(150)]
    public int TotalUpgradeCost;

    [HorizontalGroup("Row1", Width = 280)]
    [LabelWidth(230)]
    public int PurchaseCost;

    [Space(10)]
    [HorizontalGroup("Row2", Width = 200)]
    [LabelWidth(150)]
    public float StartingDamage;

    [HorizontalGroup("Row2", Width = 280)]
    [LabelWidth(230)]
    public float DamageIncreasePerUpgrade;

    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row3", Width = 200)]
    [LabelWidth(150)]
    public float StartingAttackSpeed;

    [HorizontalGroup("Row3", Width = 280)]
    [LabelWidth(230)]
    public float AttackSpeedIncreasePerUpgrade;

    [Space(10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float StartingRange;

    [HorizontalGroup("Row4", Width = 280)]
    [LabelWidth(230)]
    public float RangeIncreasePerUpgrade;
}

[System.Serializable]
public class ArcherTowerSettings : RangedTowerSettings { }

[System.Serializable]
public class MageTowerSettings : RangedTowerSettings { }

[System.Serializable]
public class CatapultTowerSettings : RangedTowerSettings
{
    [Space(10)]
    [HorizontalGroup("Row5", Width = 200)]
    [LabelWidth(150)]
    public List<ProjectileChange> ProjectileChanges;
}

[System.Serializable]
public class MilitiaTowerSettings
{
    [HorizontalGroup("Row1", Width = 200)]
    [LabelWidth(150)]
    public int TotalUpgradeCost;

    [HorizontalGroup("Row1", Width = 280)]
    [LabelWidth(230)]
    public int PurchaseCost;

    [Space(10)]
    [HorizontalGroup("Row2", Width = 200)]
    [LabelWidth(150)]
    public float UnitStartingDamage;

    [HorizontalGroup("Row2", Width = 280)]
    [LabelWidth(230)]
    public float UnitDamageIncreasePerUpgrade;

    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row3", Width = 200)]
    [LabelWidth(150)]
    public float UnitAttackSpeed;

    [HorizontalGroup("Row3", Width = 280)]
    [LabelWidth(230)]
    public float AttackSpeedIncreasePerUpgrade;

    [Space(10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float UnitStartingHealth;

    [HorizontalGroup("Row4", Width = 280)]
    [LabelWidth(230)]
    public float UnitHealthIncreasePerUpgrade;

    [Space(10)]
    [HorizontalGroup("Row5", Width = 200)]
    [LabelWidth(150)]
    public float UnitPercentHealPerSecond;

    [HorizontalGroup("Row5", Width = 280)]
    [LabelWidth(230)]
    public float PlacementRange;

    [Space(10)]
    [HorizontalGroup("Row6", Width = 200)]
    [LabelWidth(150)]
    public int UnitRespawnTime;
}

#endregion

#region Enemy Classes

[System.Serializable]
public abstract class BaseEnemySettings
{
    private bool canAttack = true;

    public virtual bool CanAttack
    {
        get => canAttack;
        set => canAttack = value;
    }

    [HorizontalGroup("Row1", Width = 200)]
    [LabelWidth(150)]
    public int Health;

    [HorizontalGroup("Row1", Width = 280)]
    [LabelWidth(230)]
    [ShowIf(nameof(CanAttack))]
    public int Damage;

    [Space(10)]
    [HorizontalGroup("Row2", Width = 200)]
    [LabelWidth(150)]
    public float Speed;

    [HorizontalGroup("Row2", Width = 280)]
    [LabelWidth(230)]
    [ShowIf(nameof(CanAttack))]
    public float AttackSpeed;

    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row3", Width = 200)]
    [LabelWidth(150)]
    public int MoneyCarried;

    [HorizontalGroup("Row3", Width = 280)]
    [LabelWidth(230)]
    [ShowIf(nameof(CanAttack))]
    public float AttackRange;
}

[System.Serializable]
public class OrcSettings : BaseEnemySettings { }

[System.Serializable]
public class WolfSettings : BaseEnemySettings { }

[System.Serializable]
public class MountedOrcSettings : BaseEnemySettings
{
    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float ChargeSpeed;

    [HorizontalGroup("Row4", Width = 280)]
    [LabelWidth(230)]
    public float ChargeDamage;

    [Space(10)]
    [HorizontalGroup("Row5", Width = 200)]
    [LabelWidth(150)]
    public float TimeBeforeCharge;
}

[System.Serializable]
public class SlimeSettings : BaseEnemySettings
{
    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public int SplitChance;

    [HorizontalGroup("Row4", Width = 280)]
    [LabelWidth(230)]
    public float JumpDistance;

    [Space(10)]
    [HorizontalGroup("Row5", Width = 200)]
    [LabelWidth(150)]
    public float AirborneTime;

    [HorizontalGroup("Row5", Width = 280)]
    [LabelWidth(230)]
    public float JumpInterval;
}

[System.Serializable]
public class SpikedSlimeSettings : BaseEnemySettings
{
    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float SpreadRange;

    [Space(10)]
    [HorizontalGroup("Row5", Width = 200)]
    [LabelWidth(150)]
    public int MiniSpikedSlimeDamage;

    [HorizontalGroup("Row5", Width = 280)]
    [LabelWidth(230)]
    public int MiniSpikedSlimeHealth;


    [HorizontalGroup("Row6", Width = 200)]
    [LabelWidth(150)]
    public float JumpDistance;

    [HorizontalGroup("Row6", Width = 280)]
    [LabelWidth(230)]
    public float AirborneTime;

    [HorizontalGroup("Row7", Width = 200)]
    [LabelWidth(150)]
    public float JumpInterval;
}

[System.Serializable]
public class BeeSettings : BaseEnemySettings { public override bool CanAttack => false; }

[System.Serializable]
public class QueenBeeSettings : BaseEnemySettings
{
    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float HiveSpawnInterval;

    [HorizontalGroup("Row4", Width = 280)]
    [LabelWidth(230)]
    public float HiveMinEndDistance;
}

[System.Serializable]
public class BeeHiveSettings : BaseEnemySettings
{
    public override bool CanAttack => false;

    [HorizontalGroup("Row1", Width = 200)]
    [LabelWidth(150)]
    public float SpawnTime;

    [Space(10)]
    [HorizontalGroup("Row2", Width = 200)]
    [LabelWidth(150)]
    public int DeathSpawnQuantity;

    [Space(10)]
    [HorizontalGroup("Row3", Width = 200)]
    [LabelWidth(150)]
    public int MaxSpawnQuantity;
}

[System.Serializable]
public class SquidSettings : BaseEnemySettings { public override bool CanAttack => false; }

[System.Serializable]
public class GiantSquidSettings : BaseEnemySettings 
{ 
    public override bool CanAttack => false;

    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float DashSpeed;
}

[System.Serializable]
public class AnglerSettings : BaseEnemySettings { }

[System.Serializable]
public class KingAnglerSettings : BaseEnemySettings { }

[System.Serializable]
public class SeagullSettings : BaseEnemySettings { public override bool CanAttack => false; }

[System.Serializable]
public class TurtleSettings : BaseEnemySettings { }

[System.Serializable]
public class ElderTurtleSettings : BaseEnemySettings
{
    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float SpecialStateDuration;

    [HorizontalGroup("Row4", Width = 280)]
    [LabelWidth(230)]
    public float ShellAbilityDPSThreshold;

    [Space(10)]
    [HorizontalGroup("Row5", Width = 200)]
    [LabelWidth(150)]
    public float ShellAbilityCooldown;
}

[System.Serializable]
public class LarvaSettings : BaseEnemySettings { }

[System.Serializable]
public class WitchSettings : BaseEnemySettings { }

[System.Serializable]
public class LizardSettings : BaseEnemySettings { }

[System.Serializable]
public class BombBatSettings : BaseEnemySettings
{
    [PropertySpace(SpaceBefore = 10)]
    [HorizontalGroup("Row4", Width = 200)]
    [LabelWidth(150)]
    public float ExplosionRadius;
}

[System.Serializable]
public class GiantLizardSettings : BaseEnemySettings { }

[System.Serializable]
public class QueenLarvaSettings : BaseEnemySettings { }

[System.Serializable]
public class TreemanSettings : BaseEnemySettings { }


#endregion


[System.Serializable]
public class GameSettings : ScriptableObject
{
    #region Tower Settings

    [FoldoutGroup("Towers")]
    [LabelWidth(200)]
    public int towerSellReturnPercentage = 40;

    [BoxGroup("Towers/Archer Tower")]
    public ArcherTowerSettings archerTowerSettings;

    [BoxGroup("Towers/Mage Tower")]
    public MageTowerSettings mageTowerSettings;

    [BoxGroup("Towers/Catapult Tower")]
    public CatapultTowerSettings catapultTowerSettings;

    [BoxGroup("Towers/Militia Tower")]
    public MilitiaTowerSettings militiaTowerSettings;

    #endregion

    #region Enemy Settings

    // Dummy property to show the foldout
    [FoldoutGroup("Enemies")]
    [LabelWidth(5)]
    [ReadOnly]
    public int UnitSettings;

    [BoxGroup("Enemies/Orc")]
    public OrcSettings orcSettings;

    [BoxGroup("Enemies/Wolf")]
    public WolfSettings wolfSettings;

    [BoxGroup("Enemies/Mounted Orc")]
    public MountedOrcSettings mountedOrcSettings;

    [BoxGroup("Enemies/Slime")]
    public SlimeSettings slimeSettings;

    [BoxGroup("Enemies/Spiked Slime")]
    public SpikedSlimeSettings spikedSlimeSettings;

    [BoxGroup("Enemies/Bee")]
    public BeeSettings beeSettings;

    [BoxGroup("Enemies/Queen Bee")]
    public QueenBeeSettings queenBeeSettings;

    [BoxGroup("Enemies/Bee Hive")]
    public BeeHiveSettings beeHiveSettings;

    [BoxGroup("Enemies/Squid")]
    public SquidSettings squidSettings;

    [BoxGroup("Enemies/Giant Squid")]
    public GiantSquidSettings giantSquidSettings;

    [BoxGroup("Enemies/Angler")]
    public AnglerSettings anglerSettings;

    [BoxGroup("Enemies/Turtle")]
    public TurtleSettings turtleSettings;

    [BoxGroup("Enemies/Seagull")]
    public SeagullSettings seagullSettings;

    [BoxGroup("Enemies/King Angler")]
    public KingAnglerSettings kingAnglerSettings;

    [BoxGroup("Enemies/Elder Turtle")]
    public ElderTurtleSettings elderTurtleSettings;

    [BoxGroup("Enemies/Larva")]
    public LarvaSettings larvaSettings;

    [BoxGroup("Enemies/Witch")]
    public WitchSettings witchSettings;

    [BoxGroup("Enemies/Lizard")]
    public LizardSettings lizardSettings;

    [BoxGroup("Enemies/Bomb Bat")]
    public BombBatSettings bombBatSettings;

    [BoxGroup("Enemies/Giant Lizard")]
    public GiantLizardSettings giantLizardSettings;

    [BoxGroup("Enemies/Queen Larva")]
    public QueenLarvaSettings queenLarvaSettings;

    [BoxGroup("Enemies/Treeman")]
    public TreemanSettings treemanSettings;

    #endregion

}

