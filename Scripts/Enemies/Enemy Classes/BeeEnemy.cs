
namespace Enemies
{
    public class BeeEnemy : FlyingEnemy
    {
        // Bees don't attack
        protected override void PlayAttackSound() { }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.beeDeathSound, transform.position);
        }
    }
}