using Core;
using System.Collections;
using UnityEngine;

namespace Towers
{
    /// <summary>
    /// Subclass of Tower, represents the Archer Tower, which fires a projectile from each archer unit sequentially
    /// </summary>
    public class ArcherTower : Tower
    {
        #region Overriden Methods

        protected override void Start()
        {
            base.Start();

            // Play the construction sound
            audioManager.PlayTowerConstructionSound(fmodEvents.archerTowerConstructionSound, 0, transform.position);
        }


        /// <summary>
        /// Overrides the default attack method, fires a projectile from each archer unit sequentially
        /// </summary>
        protected override IEnumerator AttackEnemy()
        {
            GetSurroundingEnemies();

            // Get a target from the list
            SelectTargetFromAvailable();

            // Check if the tower isn't upgrading and that there are at least one available target in range
            if (!isUpgrading && HasTarget())
            {
                if (attackingUnitIndex >= towerUnits.Count)
                {
                    attackingUnitIndex = 0;
                }

                ArcherUnit currentAttackUnit = towerUnits[attackingUnitIndex].GetComponent<ArcherUnit>();

                StartCoroutine(FireProjectile(currentAttackUnit));

                if (towerUnits.Count > 1)
                {
                    attackingUnitIndex++;
                }
            }

            yield return new WaitForSeconds(AttackSpeed);

            StartCoroutine(AttackEnemy());

            fireCoroutine = null;
            yield return null;
        }

        protected override void PlayProjectileLaunchSound(){}

        public override void UpgradeTower(int level)
        {
            base.UpgradeTower(level);

            audioManager.PlayTowerConstructionSound(fmodEvents.archerTowerConstructionSound, currentLevel, transform.position);
        }

        #endregion
    }
}

