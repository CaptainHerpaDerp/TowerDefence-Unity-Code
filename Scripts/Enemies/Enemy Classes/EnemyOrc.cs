namespace Enemies
{
    public class EnemyOrc : Enemy
    {
        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.orcAttackSound, transform.position);
        }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.orcDeathSound, transform.position);
        }
    }
}