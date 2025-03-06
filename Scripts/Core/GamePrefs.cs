using UnityEngine;
namespace Core
{
    /// <summary>
    /// Stores general game rules 
    /// </summary>
    public class GamePrefs : PersistentSingleton<GamePrefs>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        [field: SerializeField] public LayerMask RoadLayer { get; private set; }
        [field: SerializeField] public LayerMask WaterLayer { get; private set; }
        [field: SerializeField] public LayerMask EnemyLayer { get; private set; }


    }
}
