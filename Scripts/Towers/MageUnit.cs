
namespace Towers
{
    /// <summary>
    /// Tower unit created by the mage tower
    /// </summary>
    public class MageUnit : TowerUnit
    {
        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.mageFireballCastSound, transform.position);
        }

        public override void InitializeAnimationKeys()
        {

        }
    }
}