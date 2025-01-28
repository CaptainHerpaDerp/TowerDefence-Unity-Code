using UnityEngine;

namespace Core
{
    /// <summary>
    /// Contains all the sound effects in the game and methods to play them, can be accessed from any script
    /// </summary>
    public class SoundEffectManager : MonoBehaviour
    {
        public static SoundEffectManager Instance;

        #region Tower Sounds

        [Header("Tower Sounds")]

        public AudioClip smallCatapultSound;
        public AudioClip largeCatapultSound;
        public AudioClip[] catapultProjectileImpactSounds;

        public AudioClip[] bowDrawSounds;
        public AudioClip bowReleaseSound;

        public AudioClip fireballCast;
        public AudioClip fireballImpact;

        public AudioClip[] militiaTowerUpgradeSounds;
        [SerializeField] private float militiaTowerUpgradeVolume = 0.5f;

        public AudioClip[] archerTowerUpgradeSounds;
        [SerializeField] private float archerTowerUpgradeVolume = 0.5f;

        public AudioClip[] catapultTowerUpgradeSounds;
        [SerializeField] private float catapultTowerUpgradeVolume = 0.5f;

        public AudioClip[] mageTowerUpradeSounds;
        [SerializeField] private float mageTowerUpgradeVolume = 0.5f;

        public AudioClip towerSellSound;

        public AudioClip rallyPlacement;

        #endregion

        #region Unit Sounds
        [Header("Unit Sounds")]
        public AudioClip[] hitSounds;

        public AudioClip[] orcDamageSounds;
        public AudioClip[] orcAttackSounds;

        public AudioClip[] wolfDamageSounds;
        public AudioClip[] wolfAttackSounds;

        public AudioClip[] mountedOrcAttackSounds;
        public AudioClip[] mountedOrcDamageSounds;
        public AudioClip mountedOrcChargeHitSound;

        [Header("Spiked Slime Sounds")]
        public AudioClip spikedSlimeDamageSound;
        public AudioClip spikedSlimeJumpSound;
        public AudioClip spikedSlimeLandSound;
        public AudioClip[] splikedSlimeAttackSounds;

        [Header("Slime Sounds")]
        public AudioClip slimeDamageSound;
        public AudioClip slimeJumpSound;
        public AudioClip slimeLandSound;

        public AudioClip[] humanDamageSounds;
        public AudioClip[] humanAttackSounds;

        public AudioClip[] anglerAttackSounds;
        public AudioClip[] anglerDamageSounds;

        public AudioClip[] turtleAttackSounds;
        public AudioClip[] turtleDamageSounds;

        public AudioClip[] seagullDamageSounds;

        public AudioClip[] squidDamageSounds;

        public AudioClip[] beeDamageSounds;

        [Header("Elder Turtle Sounds")]
        public float elderTurtleVolume = 0.55f;
        public AudioClip elderTurtleDamageSound;
        public AudioClip elderTurtleAttackSound;

        [Header("Queen Bee Sounds")]
        public float queenBeeVolume = 0.55f;
        public AudioClip queenBeeDamageSound;
        public AudioClip queenBeeAttackSound;

        [Header("Beehive Sounds")]
        public float beeHiveVolume = 0.55f;
        public AudioClip beeHiveRaiseSound;
        public AudioClip beeHiveDeathSound;

        public AudioClip marchingLoopSound;

        #endregion

        #region Player Spell Sounds

        [Header("Player Spell Sounds")]

        public AudioClip lightningSound;

        #endregion

        #region UI Sounds

        [Header("UI Sounds")]
        public AudioClip[] scrollOpenSounds;
        public AudioClip[] scrollCloseSounds;

        public AudioClip[] confirmSounds;
        public AudioClip[] declineSounds;

        public AudioClip[] nodeCreateSounds;

        public AudioClip hoverSound;
        public AudioClip gameStartSound;

        public AudioClip waveStartSound;

        #endregion

        #region Music

        [Header("Music")]

        public AudioClip[] levelSongs;

        #endregion

        private AudioSource audioSource;
        private AudioSource musicSource;
        private AudioSource marchSource;

        public float SoundEffectVolume
        {
            get => audioSource.volume;
            set => audioSource.volume = value;
        }

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one SoundEffectManager instance in scene!");
                Destroy(gameObject);
                return;
            }

            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            musicSource = gameObject.AddComponent<AudioSource>();

            marchSource = gameObject.AddComponent<AudioSource>();

            //PlayMarchingLoop();
        }

        #region Tower Sound Methods

        public void PlaySmallCatapultSound()
        {
            PlaySoundEffect(smallCatapultSound);
        }

        public void PlayLargeCatapultSound()
        {
            PlaySoundEffect(largeCatapultSound);
        }

        public void PlayCatapultProjectileImpactSound()
        {
            // Randomly play one of the impact sounds
            PlaySoundEffect(catapultProjectileImpactSounds[Random.Range(0, catapultProjectileImpactSounds.Length)]);
        }

        public void PlayHitSound()
        {
            // Randomly play one of the hit sounds
            PlaySoundEffect(hitSounds[Random.Range(0, hitSounds.Length)]);
        }

        public void PlayBowDrawSound()
        {
            PlaySoundEffect(bowDrawSounds[Random.Range(0, bowDrawSounds.Length)]);
        }

        public void PlayBowReleaseSound()
        {
            PlaySoundEffect(bowReleaseSound);
        }

        public void PlayFireballCastSound()
        {
            PlaySoundEffect(fireballCast);
        }

        public void PlayFireballImpactSound()
        {
            PlaySoundEffect(fireballImpact);
        }

        public void PlayMilitiaTowerUpgradeSound(int level)
        {
            if (level - 1 < 0 || level - 1 >= militiaTowerUpgradeSounds.Length)
            {
                Debug.LogWarning("Militia tower upgrade sound index out of range. Unable to play sound.");
                return;
            }

            PlaySoundEffect(militiaTowerUpgradeSounds[level - 1], percentChangeMax: 3, volume: militiaTowerUpgradeVolume);
        }

        public void PlayArcherTowerUpgradeSound(int level)
        {
            if (level - 1 < 0 || level - 1 >= archerTowerUpgradeSounds.Length)
            {
                Debug.LogWarning("Archer tower upgrade sound index out of range. Unable to play sound.");
                return;
            }

            PlaySoundEffect(archerTowerUpgradeSounds[level - 1], percentChangeMax: 3, volume: archerTowerUpgradeVolume);
        }

        public void PlayCatapultTowerUpgradeSound(int level)
        {
            if (level - 1 < 0 || level - 1 >= catapultTowerUpgradeSounds.Length)
            {
                Debug.LogWarning("Catapult tower upgrade sound index out of range. Unable to play sound.");
                return;
            }

            PlaySoundEffect(catapultTowerUpgradeSounds[level - 1], percentChangeMax: 3, volume: catapultTowerUpgradeVolume);
        }

        public void PlayMageTowerUpgradeSound(int level)
        {
            if (level - 1 < 0 || level - 1 >= mageTowerUpradeSounds.Length)
            {
                Debug.LogWarning("Mage tower upgrade sound index out of range. Unable to play sound.");
                return;
            }

            PlaySoundEffect(mageTowerUpradeSounds[level - 1], percentChangeMax: 3, volume: mageTowerUpgradeVolume);
        }

        public void PlayTowerSellSound()
        {
            PlaySoundEffect(towerSellSound);
        }

        public void PlayRallyPointPlacement()
        {
            PlaySoundEffect(rallyPlacement);
        }

        #endregion 

        #region Unit Sound Methods
        public void PlayOrcHitSound()
        {
            PlaySoundEffect(orcDamageSounds[Random.Range(0, orcDamageSounds.Length)]);
        }

        public void PlayOrcAttackSound()
        {
            PlaySoundEffect(orcAttackSounds[Random.Range(0, orcAttackSounds.Length)]);
        }

        public void PlayWolfHitSound()
        {
            PlaySoundEffect(wolfDamageSounds[Random.Range(0, wolfDamageSounds.Length)]);
        }

        public void PlayWolfAttackSound()
        {
            PlaySoundEffect(wolfAttackSounds[Random.Range(0, wolfAttackSounds.Length)]);
        }

        public void PlayMountedOrcAttackSound()
        {
            PlaySoundEffect(mountedOrcAttackSounds[Random.Range(0, mountedOrcAttackSounds.Length)]);
        }

        public void PlayMountedOrcDamageSound()
        {
            PlaySoundEffect(mountedOrcDamageSounds[Random.Range(0, mountedOrcDamageSounds.Length)]);
        }

        public void PlayMountedOrcChargeHitSound()
        {
            PlaySoundEffect(mountedOrcChargeHitSound);
        }

        public void PlayHumanAttackSound()
        {
            PlaySoundEffect(humanAttackSounds[Random.Range(0, humanAttackSounds.Length)]);
        }

        public void PlayHumanDamageSound()
        {
            PlaySoundEffect(humanDamageSounds[Random.Range(0, humanDamageSounds.Length)]);
        }

        public void PlaySpikedSlimeDamageSound()
        {
            PlaySoundEffect(spikedSlimeDamageSound);
        }

        public void PlaySpikedSlimeJumpSound()
        {
            PlaySoundEffect(spikedSlimeJumpSound);
        }

        public void PlaySpikedSlimeLandSound()
        {
            PlaySoundEffect(spikedSlimeLandSound);
        }

        public void PlaySlimeDamageSound()
        {
            PlaySoundEffect(slimeDamageSound);
        }

        public void PlaySlimeJumpSound()
        {
            PlaySoundEffect(slimeJumpSound);
        }

        public void PlaySlimeLandSound()
        {
            PlaySoundEffect(slimeLandSound);
        }

        public void PlayAnglerAttackSound()
        {
            PlaySoundEffect(anglerAttackSounds[Random.Range(0, anglerAttackSounds.Length)], volume: 0.1f);
        }

        public void PlayAnglerDamageSound()
        {
            PlaySoundEffect(anglerDamageSounds[Random.Range(0, anglerDamageSounds.Length)], volume: 0.55f);
        }

        public void PlayTurtleAttackSound()
        {
            PlaySoundEffect(turtleAttackSounds[Random.Range(0, turtleAttackSounds.Length)], volume: 0.5f);
        }

        public void PlayTurtleDamageSound()
        {
            PlaySoundEffect(turtleDamageSounds[Random.Range(0, turtleDamageSounds.Length)], volume: 0.55f);
        }

        public void PlayElderTurtleDamageSound()
        {
            PlaySoundEffect(elderTurtleDamageSound, volume: elderTurtleVolume);
        }

        public void PlayElderTurtleAttackSound()
        {
            PlaySoundEffect(elderTurtleAttackSound, volume: elderTurtleVolume);
        }

        public void PlayQueenBeeDamageSound()
        {
            PlaySoundEffect(queenBeeDamageSound, volume: queenBeeVolume);
        }

        public void PlayQueenBeeAttackSound()
        {
            PlaySoundEffect(queenBeeAttackSound, volume: queenBeeVolume);
        }

        public void PlayBeehiveRaiseSound()
        {
            PlaySoundEffect(beeHiveRaiseSound, volume: beeHiveVolume);
        }

        public void PlayBeehiveDeathSound()
        {
            PlaySoundEffect(beeHiveDeathSound, volume: beeHiveVolume);
        }

        public void PlaySeagullDamageSound()
        {
            PlaySoundEffect(seagullDamageSounds[Random.Range(0, seagullDamageSounds.Length)], volume: 0.45f);
        }

        public void PlaySquidDamageSound()
        {
            PlaySoundEffect(squidDamageSounds[Random.Range(0, squidDamageSounds.Length)], volume: 0.4f);
        }

        public void PlayBeeDamageSound()
        {
            PlaySoundEffect(beeDamageSounds[Random.Range(0, beeDamageSounds.Length)], volume: 0.4f);
        }

        public void PlaySpikedSlimeAttackSound()
        {
            PlaySoundEffect(splikedSlimeAttackSounds[Random.Range(0, splikedSlimeAttackSounds.Length)], volume: 0.4f);
        }

        #endregion

        #region Player Spell Sound Methods

        public void PlayLightningSound()
        {
            PlaySoundEffect(lightningSound, volume: 0.6f);
        }

        #endregion

        #region UI Sound Methods

        public void PlayScrollOpenSound()
        {
            PlaySoundEffect(scrollOpenSounds[Random.Range(0, scrollOpenSounds.Length)]);
        }

        public void PlayScrollCloseSound()
        {
            PlaySoundEffect(scrollCloseSounds[Random.Range(0, scrollCloseSounds.Length)]);
        }

        public void PlayConfirmSound()
        {
            PlaySoundEffect(confirmSounds[Random.Range(0, confirmSounds.Length)]);
        }

        public void PlayDeclineSound()
        {
            PlaySoundEffect(declineSounds[Random.Range(0, declineSounds.Length)]);
        }

        public void PlayHoverSound()
        {
            PlaySoundEffect(hoverSound);
        }

        public void PlayGameStartSound()
        {
            PlaySoundEffect(gameStartSound);
        }

        public void PlayNodeCreateSound()
        {
            PlaySoundEffect(nodeCreateSounds[Random.Range(0, nodeCreateSounds.Length)]);
        }

        #endregion

        #region Music Methods

        public void PlayLevelSong(int level)
        {
            if (levelSongs.Length <= level)
            {
                Debug.LogWarning("Level index out of range. Unable to play level song.");
                return;
            }

            else if (levelSongs[level] == null)
            {
                Debug.LogWarning("Level song is null. Unable to play level song.");
                return;
            }

            PlayMusic(levelSongs[level]);
        }

        #endregion

        #region World Sound Methods

        public void PlayWaveStartSound()
        {
            PlaySoundEffect(waveStartSound);
        }

        #endregion

        /// <summary>
        ///  Plays the given sound effect with the option to randomize its pitch by a maximum given factor 
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="randomizePitch"></param>
        /// <param name="percentChangeMax"></param>
        private void PlaySoundEffect(AudioClip clip, bool randomizePitch = true, float percentChangeMax = 10, float volume = 1)
        {
            float actualPercentChange = percentChangeMax / 100;

            if (clip != null)
            {
                if (randomizePitch)
                {
                    audioSource.pitch = Random.Range(1 - actualPercentChange, 1 + actualPercentChange);
                }
                else
                {
                    audioSource.pitch = 1f;
                }

                audioSource.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.LogWarning("AudioClip is null. Unable to play sound effect.");
            }
        }

        private void PlayMusic(AudioClip clip)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.pitch = 1f;
            musicSource.Play();
        }

        public void PlayMarchingLoop()
        {
            marchSource.clip = marchingLoopSound;
            marchSource.loop = true;
            marchSource.pitch = 1f;
            marchSource.Play();
        }
    }

}