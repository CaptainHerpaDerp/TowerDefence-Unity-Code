using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enemies;
using Militia;
using AudioManagement;

namespace Towers
{
    /// <summary>
    /// A point where assigned militia units will move to and will be assigned to combat with enemies that enter this area's influence
    /// </summary>
    public class RallyPoint : MonoBehaviour
    {
        // The units that are assigned to the rally point
        public List<MilitiaUnit> rallyPointUnits = new();

        // The marks where the units will be positioned
        private List<Transform> unitMarks = new();

        // The (main) active combats that are currently taking place
        private Dictionary<MilitiaUnit, Enemy> activeCombats = new();

        [Header("Enemy Detection")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float enemyDetectionRadius = 0.75f;
        [SerializeField] private float enemyCheckInterval = 0.1f;

        // Singletons
        private AudioManager audioManager;
        private FMODEvents fmodEvents;

        private void Start()
        {
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;
        }

        public void Initialize()
        {
            foreach (var transform in transform.GetComponentsInChildren<Transform>())
            {
                if (transform == this.transform)
                {
                    continue;
                }

                unitMarks.Add(transform);
            }

            StartCoroutine(AssignUnitsToCombat());
            StartCoroutine(CheckActiveCombats());
            StartCoroutine(GetEnemiesInArea());
        }

        /// <summary>
        /// Assigns available militia units to enter combat with enemies and marks this as a "Main Combat". Other militia units that are not assigned to a combat will assist the main combat until another available enemy is found.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AssignUnitsToCombat()
        {
            while (true)
            {             
                // Assisting target selection loop
                foreach (var combat in activeCombats)
                {
                    // The combat target may have been destroyed. If so, remove the combat from the list
                    if (combat.Value == null || combat.Value.IsDead())
                    {
                        //Debug.Log("Combat target is null or dead");
                        activeCombats.Remove(combat.Key);

                        // Free the militia unit from the combat
                        if (combat.Key != null)
                        {
                            combat.Key.ExitAttackState();
                        }

                        break;
                    }

                    // If either unit is dead, skip 
                    if (combat.Key.IsDead() || combat.Value.gameObject == null || combat.Value.IsDead())
                    {
                        continue;
                    }

                    foreach (var availableUnit in rallyPointUnits)
                    {
                        // Skip if the unit is already engaged in combat
                        if (activeCombats.Keys.Contains(availableUnit))
                        {
                            continue;
                        }

                        // Skip if the unit is dead
                        if (availableUnit.IsDead())
                        {
                            continue;
                        }

                        // Skip if the unit already has a combat target
                        if (availableUnit.HasCombatTarget())
                        {
                            continue;
                        }

                        availableUnit.SetCombatTarget(combat.Value);
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Assigns an available militia unit to combat with the given enemy
        /// </summary>
        /// <param name="enemy"></param>
        private void AssignUnitToCombat(Enemy enemy)
        {
            MilitiaUnit militiaUnit = null;

            // Get the first available unit
            foreach (var unit in rallyPointUnits)
            {
                if (unit == null || unit.IsDead())
                {
                    continue;
                }

                if (activeCombats.Keys.Contains(unit))
                {
                    continue;
                }

                militiaUnit = unit;
            }

            if (militiaUnit == null || militiaUnit.IsDead())
            {
                Debug.Log("No available units");
                return;
            }

            militiaUnit.SetCombatTarget(enemy);
            enemy.SetCombatTarget(militiaUnit);

            activeCombats.Add(militiaUnit, enemy);
        }


        /// <summary>
        /// Checks the active combats and removes them if either the unit or the enemy is dead
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckActiveCombats()
        {
            yield return new WaitForSeconds(1);

            while (true)
            {
                // create a copy of the dictionary to avoid modifying it while iterating
                var activeCombatsCopy = new Dictionary<MilitiaUnit, Enemy>(this.activeCombats);

                foreach (var combat in activeCombatsCopy)
                {
                    // The combat target may have been destroyed. If so, remove the combat from the list
                    if (combat.Value == null || combat.Value.IsDead())
                    {
                        //Debug.Log("Combat target is null or dead");
                        activeCombats.Remove(combat.Key);

                        // Free the militia unit from the combat
                        if (combat.Key != null)
                        {
                            combat.Key.ExitAttackState();
                        }

                        break;
                    }

                    /* If the militia unit is dead, we need to see if there are any other militia units "assisting" the combat, 
                     * if so, we need to assign them as the main combatant in the active combats list */

                    if (combat.Key.IsDead())
                    {
                        Enemy enemy = combat.Value;

                        // Remove the dead unit from the active combats list
                        activeCombats.Remove(combat.Key);

                        foreach (var unit in rallyPointUnits)
                        {
                            // If the unit is dead, skip
                            if (unit == null || unit.IsDead())
                            {
                                continue;
                            }

                            // If the unit is already assigned as a main combatant, skip
                            if (activeCombats.Keys.Contains(unit))
                            {
                                continue;
                            }

                            // Check if the unit's target is the same as the dead unit's target
                            if (unit.GetCombatTarget() != enemy)
                            {
                                continue;
                            }

                            Debug.Log("Reassigned enemy to substitute");

                            // change the dictionary key to the new unit
                            activeCombats.Add(unit, combat.Value);

                            // Assign the combat target to the new unit
                            enemy.SetCombatTarget(unit);

                            break;
                        }
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Changes the position of the rally point and the position marks of the assigned militia units
        /// </summary>
        /// <param name="newPos"></param>
        public void ChangePosition(Vector3 newPos)
        {
            transform.position = newPos;

            if (rallyPointUnits.Count == 0)
            {
                return;
            }

            audioManager.PlayOneShot(fmodEvents.militiaRallyPlacementSound, transform.position);
            
            // Set the new position marks for the units
            for (int i = 0; i < rallyPointUnits.Count; i++)
            {
                rallyPointUnits[i].SetPositionMark(unitMarks[i]);
            }
        }


        /// <summary>
        /// Periodically checks for enemies within the area of the rally point
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetEnemiesInArea()
        {
            while (true)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRadius, enemyLayer);

                HashSet<Enemy> detectedEnemies = new();

                List<RallyPoint> rallyPoints = GetAllRallyPoints();

                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent<Enemy>(out var enemy))
                    {
                        detectedEnemies.Add(enemy);

                        // Do not add the enemy to the list if all rally units are dead
                        if (!AllRallyUnitsDead())
                        {
                            bool enemyIsInCombat = false;

                            /* In order to avoid multiple rally points assigning their militia units to the same enemy, 
                             * we need to check if the enemy is already in combat in another rally point */

                            // Check if the enemy is already in combat in another rally point
                            foreach (var rallyPoint in rallyPoints)
                            {
                                if (rallyPoint.IsEnemyClaimed(enemy))
                                {
                                    enemyIsInCombat = true;
                                    break;
                                }
                            }

                            // If the enemy is not in combat, assign it to a militia unit
                            if (HasAvailableCombatant() && !enemyIsInCombat && !enemy.HasCombatTarget() && !enemy.IsDead() && enemy.EngagesInCombat && !activeCombats.Values.Contains(enemy))
                            {
                                AssignUnitToCombat(enemy);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Collider does not have an enemy component");
                    }
                }

                yield return new WaitForSeconds(enemyCheckInterval);
            }
        }


        private List<RallyPoint> GetAllRallyPoints()
        {
            List<RallyPoint> rallyPoints = FindObjectsOfType<RallyPoint>().ToList();

            if (rallyPoints.Contains(this))
            {
                rallyPoints.Remove(this);
            }

            return rallyPoints;
        }

        private bool AllRallyUnitsDead()
        {
            foreach (var unit in rallyPointUnits)
            {
                if (!unit.IsDead())
                {
                    return false; 
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the given enemy is engaged in combat 
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private bool IsEnemyClaimed(Enemy enemy)
        {
            if (activeCombats.Values.Contains(enemy))
            {
                Debug.Log("Enemy is in combat, cannot add");
            }

            return activeCombats.Values.Contains(enemy);
        }

        /// <summary>
        /// Returns true if a militia unit is not engaged in combat and marked as the main combatant
        /// </summary>
        private bool HasAvailableCombatant()
        {
            foreach (var unit in rallyPointUnits)
            {
                if (!activeCombats.Keys.Contains(unit))
                {
                    return true;
                }
            }

            return false;
        }     
    }
}