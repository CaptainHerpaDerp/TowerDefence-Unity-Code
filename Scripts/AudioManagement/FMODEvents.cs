using Core;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AudioManagement
{
    public class FMODEvents : PersistentSingleton<FMODEvents>
    {
        #region Unit Sounds

        [FoldoutGroup("Unit Sounds")]
        [FoldoutGroup("Unit Sounds/Human")]
        [field: SerializeField] public EventReference humanDeathSound { get; private set; }
        [field: SerializeField] public EventReference humanAttackSound { get; private set; }

        [FoldoutGroup("Unit Sounds/Orc")]
        [field: SerializeField] public EventReference orcDeathSound { get; private set; }
        [field: SerializeField] public EventReference orcAttackSound { get; private set; }
        [field: SerializeField] public EventReference mountedOrcAttackSound { get; private set; }
        [field: SerializeField] public EventReference mountedOrcDeathSound { get; private set; }
        [field: SerializeField] public EventReference mountedOrcChargeHitSound { get; private set; }

        [FoldoutGroup("Unit Sounds/Wolf")]
        [field: SerializeField] public EventReference wolfDeathSound { get; private set; }
        [field: SerializeField] public EventReference wolfAttackSound { get; private set; }

        [FoldoutGroup("Unit Sounds/Spiked Slime")]
        [field: SerializeField] public EventReference spikedSlimeDeathSound { get; private set; }
        [field: SerializeField] public EventReference spikedSlimeJumpSound { get; private set; }
        [field: SerializeField] public EventReference spikedSlimeLandSound { get; private set; }
        [field: SerializeField] public EventReference spikedSlimeAttackSound { get; private set; }


        [FoldoutGroup("Unit Sounds/Slime")]
        [field: SerializeField] public EventReference slimeDeathSound { get; private set; }
        [field: SerializeField] public EventReference slimeJumpSound { get; private set; }
        [field: SerializeField] public EventReference slimeLandSound { get; private set; }

        [FoldoutGroup("Unit Sounds/Sea Creatures")]
        [field: SerializeField] public EventReference anglerAttackSound { get; private set; }
        [field: SerializeField] public EventReference anglerDeathSound { get; private set; }
        [field: SerializeField] public EventReference turtleAttackSound { get; private set; }
        [field: SerializeField] public EventReference turtleDeathSound { get; private set; }
        [field: SerializeField] public EventReference elderTurtleDeathSound { get; private set; }
        [field: SerializeField] public EventReference elderTurtleAttackSound { get; private set; }
        [field: SerializeField] public EventReference seagullDeathSound { get; private set; }
        [field: SerializeField] public EventReference squidDeathSound { get; private set; }

        [FoldoutGroup("Unit Sounds/Insects")]
        [field: SerializeField] public EventReference queenBeeDeathSound { get; private set; }
        [field: SerializeField] public EventReference queenBeeAttackSound { get; private set; }
        [field: SerializeField] public EventReference beeHiveRaiseSound { get; private set; }
        [field: SerializeField] public EventReference beeHiveDeathSound { get; private set; }
        [field: SerializeField] public EventReference beeDeathSound { get; private set; }

        #endregion

        #region Spell Sounds

        [field: SerializeField] public EventReference lightningSound { get; private set; }

        #endregion

        #region UI Sounds

        [FoldoutGroup("UI Sounds")]
        [field: SerializeField] public EventReference scrollOpenSound { get; private set; }
        [field: SerializeField] public EventReference scrollCloseSound { get; private set; }
        [field: SerializeField] public EventReference confirmSound { get; private set; }
        [field: SerializeField] public EventReference declineSound { get; private set; }
        [field: SerializeField] public EventReference nodeCreateSound { get; private set; }
        [field: SerializeField] public EventReference hoverSound { get; private set; }
        [field: SerializeField] public EventReference gameStartSound { get; private set; }
        [field: SerializeField] public EventReference waveStartSound { get; private set; }

        #endregion

        #region Tower Sounds

        [FoldoutGroup("Towers")]
        [field: SerializeField, BoxGroup("Towers")] public EventReference towerSellSound { get; private set; }

        [field: SerializeField, BoxGroup("Towers/Archer")] public EventReference archerTowerConstructionSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Archer")] public EventReference archerBowDrawSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Archer")] public EventReference archerProjectileHitSound { get; private set; }


        [field: SerializeField, BoxGroup("Towers/Militia")] public EventReference militiaTowerConstructionSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Militia")] public EventReference militiaRallyPlacementSound { get; private set; }


        [field: SerializeField, BoxGroup("Towers/Mage")] public EventReference mageTowerConstructionSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Mage")] public EventReference mageFireballCastSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Mage")] public EventReference mageFireballHitSound { get; private set; }


        [field: SerializeField, BoxGroup("Towers/Catapult")] public EventReference catapultTowerConstructionSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Catapult")] public EventReference smallCatapultFireSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Catapult")] public EventReference largeCatapultFireSound { get; private set; }
        [field: SerializeField, BoxGroup("Towers/Catapult")] public EventReference catapultProjectileImpactSound { get; private set; }

        #endregion
    }
}
