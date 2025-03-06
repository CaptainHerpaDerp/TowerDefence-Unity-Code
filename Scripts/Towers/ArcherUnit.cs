
namespace Towers
{
    /// <summary>
    /// Tower units created by the Archer Tower
    /// </summary>
    public class ArcherUnit : TowerUnit
    {
        public override void InitializeAnimationKeys()
        { 
            ATTACK_UP = "Attack_U";
            ATTACK_DOWN = "Attack_D";
            ATTACK_LEFT = "Attack_L";
            ATTACK_RIGHT = "Attack_R";
        }

        protected override void PlayAttackSound()
        {   
            audioManager.PlayOneShot(fmodEvents.archerBowDrawSound, transform.position);
        }
    }
}
