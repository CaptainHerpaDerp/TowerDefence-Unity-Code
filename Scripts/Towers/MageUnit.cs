
namespace Towers
{
    /// <summary>
    /// Tower unit created by the mage tower
    /// </summary>
    public class MageUnit : TowerUnit
    {
        protected override void PlayAttackSound()
        {
            soundEffectManager.PlayFireballCastSound();
        }

        public override void InitializeAnimationKeys()
        {

        }
    }
}