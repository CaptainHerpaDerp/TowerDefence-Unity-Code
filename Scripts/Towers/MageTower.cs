namespace Towers
{
    public class MageTower : Tower
    {
        #region Overriden Methods

        protected override void Start()
        {
            base.Start();

            audioManager.PlayTowerConstructionSound(fmodEvents.mageTowerConstructionSound, 0, transform.position);
        }

        public override void UpgradeTower(int level)
        {
            base.UpgradeTower(level);

            audioManager.PlayTowerConstructionSound(fmodEvents.mageTowerConstructionSound, level, transform.position);
        }

        #endregion
    }
}
