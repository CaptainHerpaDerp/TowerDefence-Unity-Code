
namespace Enemies
{
    /// <summary>
    /// A flying enemy that does not engage in combat
    /// </summary>
    public abstract class FlyingEnemy : Enemy
    {
        protected override void Start()
        {
            EngagesInCombat = false;
            base.Start();
        }
    }
}