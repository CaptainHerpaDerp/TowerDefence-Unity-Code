namespace Enemies
{
    public class WolfEnemy : Enemy
    {
        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.wolfAttackSound, transform.position);
        }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.wolfDeathSound, transform.position);
        }
    }
}
