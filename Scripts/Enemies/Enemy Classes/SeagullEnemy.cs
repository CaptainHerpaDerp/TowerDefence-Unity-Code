namespace Enemies
{
    public class SeagullEnemy : FlyingEnemy
    {
        // Seagull doesn't attack
        protected override void PlayAttackSound() { }
        
        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.seagullDeathSound, transform.position);
        }
    }
}
