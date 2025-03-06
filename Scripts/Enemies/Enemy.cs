using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Character;
using Core;

namespace Enemies
{
    /// <summary>
    /// Identifier for the type of enemy
    /// </summary>
    public enum EnemyType
    {
        Orc,
        Wolf,
        Slime,
        MountedOrc,
        SpikedSlime,
        Bee,
        QueenBee,
        Squid,
        Angler,
        Turtle,
        Gull,
        KingAngler,
        GiantSquid,
        ElderTurtle,
        Larva,
        Witch,
        Lizard,
        BombBat,
        GiantLizard,
        QueenLarva,
        Treeman,
        BeeHive
    }

    public enum TraverseType
    {
        Road,
        Water,
        Hybrid
    }

    /// <summary>
    /// The base class for all enemies in the game. Creates a path using the RoadPathCreator and moves the enemy along the path. Is the target of all player towers. Able to attack militia units if set to do so.
    /// </summary>
    public class Enemy : Character
    {
        RoadPathCreator roadPathCreator;

        [SerializeField] private EnemyType enemyType;
        [SerializeField] private TraverseType traverseType = TraverseType.Road;
        public TraverseType TraverseType => traverseType;

        [HideInInspector] public float moneyCarried;

        [SerializeField] protected float minDistanceToWaypoint = 0.2f;

        private float totalDistanceToTravel;
        public int CurrentWaypointIndex { get; protected set; } = 0;

        protected List<Vector3> roadWaypoints = new List<Vector3>();

        private bool hasInstantKill;

        [HideInInspector] public Transform EndPoint;

        // The ring object that will be enabled when the enemy is selected for whatever reason
        [SerializeField] private GameObject selectionRing;

        [SerializeField] private bool kill;
        
        protected override void Start()
        {
            eventBus = EventBus.Instance;

            //eventBus.Publish("EnemyCreated", OnEnemyCreated);

            roadPathCreator = RoadPathCreator.Instance;

            if (EndPoint == null)
            {
                EndPoint = GameObject.FindGameObjectWithTag("EndPoint").transform;
            }

            roadWaypoints = roadPathCreator.CreatePath(transform.position, EndPoint.position);

            StartCoroutine(TravelWaypoints());

            // Set the total distance to travel
            totalDistanceToTravel = 0;

            for (int i = 0; i < roadWaypoints.Count - 1; i++)
            {
                totalDistanceToTravel += Vector2.Distance(roadWaypoints[i], roadWaypoints[i + 1]);
            }

            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (kill)
            {
                IntakeDamage(1000);
                kill = false;
            }
        }

        /// <summary>
        /// The passed enemy will be instantiated within the level event manager (listening to death, EndPoint reached, etc.)
        /// </summary>
        /// <param name="newEnemy"></param>
        protected void InstantiateEnemy(Enemy newEnemy)
        {
            if (eventBus == null)
            {
                eventBus = EventBus.Instance;
            }

            eventBus.Publish("EnemyCreated", newEnemy);
        }

        public void ForceStart(RoadPathCreator roadPathCreator, Vector3 EndPoint)
        {
            roadWaypoints = roadPathCreator.CreatePath(transform.position, EndPoint);
        }

        public Vector3 GetLastWaypointPosition()
        {
            return roadWaypoints[^1];
        }

        /// <summary>
        /// Return the combined distance from the current position to the next waypoint and the distance from the next waypoint to the last waypoint.
        /// </summary>
        /// <returns></returns>
        public virtual float GetTotalDistanceToTravel()
        {
            float remainingDistance = 0;

            // Add the distance from the current position to the next waypoint
            if (CurrentWaypointIndex < roadWaypoints.Count - 1)
            {
                remainingDistance += Vector2.Distance(transform.position, roadWaypoints[CurrentWaypointIndex]);
            }

            // Add the distance from the next waypoint to the last waypoint
            for (int i = CurrentWaypointIndex; i < roadWaypoints.Count - 1; i++)
            {
                remainingDistance += Vector2.Distance(roadWaypoints[i], roadWaypoints[i + 1]);
            }

            return remainingDistance;
        }

        /// <summary>
        /// Returns true if the unit is on the given ground layer
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsOnGround()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, 0, GamePrefs.Instance.RoadLayer).collider != null;
        }

        protected virtual bool IsOverWater()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, 0, GamePrefs.Instance.WaterLayer).collider != null;
        }

        public void SetWaypointIndex(int index)
        {
            CurrentWaypointIndex = index;
        }

        public void SetWaypointList(List<Vector3> list)
        {
            roadWaypoints = list;
        }

        public void OnInstantKillChanged()
        {
            hasInstantKill = !hasInstantKill;
        }

        public void SetInstantKill(bool value)
        {
            hasInstantKill = value;
        }

        /// <summary>
        /// Continuously moves the enemy along the path until it reaches the last waypoint or it is engaged in combat.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator TravelWaypoints()
        {
            while (CurrentWaypointIndex < roadWaypoints.Count)
            {
                if (IsDead() || combatTarget != null)
                {
                    yield break;
                }

                // Set the next waypoint as the target
                Vector3 nextWaypoint = roadWaypoints[CurrentWaypointIndex];

                // Check if the unit has reached the waypoint
                while (Vector2.Distance(transform.position, nextWaypoint) > minDistanceToWaypoint)
                {
                    // Move towards the waypoint

                    if (IsDead() || combatTarget != null)
                    {
                        yield break;
                    }

                    movementTargetPos = nextWaypoint;
                    yield return null;
                }

                // Reduce the total distance to travel by the distance between the current and previous waypoints
                if (CurrentWaypointIndex > 0)
                    totalDistanceToTravel -= Vector2.Distance(roadWaypoints[CurrentWaypointIndex - 1], nextWaypoint);

                // Increment the waypoint index
                CurrentWaypointIndex++;
            }

            // The loop has exited, and the unit has reached the last waypoint
            HandleLastWaypointReached();
        }

        // After exiting the attack state, the unit should continue to travel to the next waypoint but not go to a waypoint that is in the opposite direction of the next waypoint
        private void IncreaseWaypointIndex()
        {
            if (CurrentWaypointIndex + 2 < roadWaypoints.Count)
            {
                CurrentWaypointIndex += 2;
                return;
            }
            else if (CurrentWaypointIndex + 1 < roadWaypoints.Count)
            {
                CurrentWaypointIndex += 1;
                return;
            }
        }

        /// <summary>
        /// Overrides the intake damage method to handle the instant kill modifier.
        /// </summary>
        /// <param name="amount"></param>
        public override void IntakeDamage(float amount)
        {
            if (hasInstantKill)
            {
                CurrentHealth = 0;
                StartCoroutine(EnterDeathState());
                return;
            }

            base.IntakeDamage(amount);
        }

        /// <summary>
        /// Exits the attack state and re-calls the waypoint travel coroutine.
        /// </summary>
        public override void ExitAttackState()
        {
            base.ExitAttackState();
           // IncreaseWaypointIndex();
            StartCoroutine(TravelWaypoints());
        }

        /// <summary>
        /// Handles the event of the enemy reaching the last waypoint. Destroys the enemy and calls the OnEndPointReached event.
        /// </summary>
        private void HandleLastWaypointReached()
        {
            eventBus.Publish("EnemyEndPointReached");
            //OnEndPointReached?.Invoke();
            Destroy(gameObject);
        }

        public void EnableSelectionRing()
        {
            selectionRing.SetActive(true);
        }

        public void DisableSelectionRing()
        {
            selectionRing.SetActive(false);
        }

        protected override void PlayDeathSound()
        {
            throw new System.NotImplementedException();
        }

        protected override void PlayAttackSound()
        {
            throw new System.NotImplementedException();
        }
    }
}