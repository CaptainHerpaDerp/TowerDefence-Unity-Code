namespace Towers
{
    public class MageTower : Tower
    {
        #region Overriden Methods

        protected override void Start()
        {
            base.Start();

            soundEffectManager.PlayMageTowerUpgradeSound(1);
        }

        public override void UpgradeTower(int level)
        {
            base.UpgradeTower(level);

            soundEffectManager.PlayMageTowerUpgradeSound(currentLevel);
        }

        #endregion
    }
}
