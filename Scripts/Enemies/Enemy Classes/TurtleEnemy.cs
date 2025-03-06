namespace Enemies
{
    public class TurtleEnemy : Enemy
    {
        protected override void PlayAttackSound()
        {
            audioManager.PlayOneShot(fmodEvents.turtleAttackSound, transform.position);
        }

        protected override void PlayDeathSound()
        {
            audioManager.PlayOneShot(fmodEvents.turtleDeathSound, transform.position);
        }
    }
}