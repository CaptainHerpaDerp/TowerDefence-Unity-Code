using Core.Character;

namespace Towers
{
    public class ArrowProjectile : Projectile
    {
        protected override void PlayImpactSound()
        {
            audioManager.PlayOneShot(fmodEvents.archerProjectileHitSound, transform.position);
        }
    }
}