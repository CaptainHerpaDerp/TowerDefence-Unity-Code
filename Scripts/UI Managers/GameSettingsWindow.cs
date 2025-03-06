using AudioManagement;
using Core;
using Saving;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIElements;


namespace UIManagement
{
    public class GameSettingsWindow : Singleton<GameSettingsWindow>
    {
        [BoxGroup("Button References"), SerializeField] protected Button exitButton, restartButton, resumeButton;

        [BoxGroup("Component References"), SerializeField] protected ExpandingScrollVertical expandingScrollVertical;

        [InfoBox("We need to have references to these groups so that we can disable them if we are using WebGL builds")]
        [BoxGroup("Resolution Groups"), SerializeField] protected GameObject resolutionGroup;
        [BoxGroup("Resolution Groups"), SerializeField] protected GameObject fullscreenToggleGroup;

        [Header("Resolution Elements")]
        [SerializeField] protected Button resolutionRight, resolutionLeft;
        [SerializeField] protected TextMeshProUGUI resolutionText;

        [Header("Fullscreen Elements")]
        [SerializeField] protected Button fullscreenButtonOn, fullscreenButtonOff;

        [Header("Sound Effect Elements")]
        [SerializeField] protected Slider soundEffectSlider;
        [SerializeField] protected Slider musicSlider;

        protected bool settingsEnabled = false;
        protected int resolution = 0;

        protected int nativeX, nativeY;

        protected bool isFullscreen => Screen.fullScreen;

        protected List<Resolution> supportedResolutions;

        // Singletons
        protected EventBus eventBus;
        protected SaveManager saveManager;
        protected AudioManager audioManager;
        protected FMODEvents fmodEvents;

        #region Unity Functions

        protected virtual void Start()
        {
            // Singleton Assignment
            eventBus = EventBus.Instance;
            saveManager = SaveManager.Instance;
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;

            Initialize();
        }

        protected virtual void Update()
        {
            // Check for changes to the sound effect volume
            if (audioManager.MasterVolume != soundEffectSlider.value)
            {
                audioManager.SetMasterVolume(soundEffectSlider.value);
            }

            //if (saveData.settingsData.soundEffectVolume != musicSlider.value)
            //{
            //    saveData.settingsData.musicVolume = musicSlider.value;
            //}
        }

        // When the scene is changed, or the game is closed, save the settings data
        protected virtual void OnApplicationQuit()
        {
            saveManager.SetSoundEffectVolume(soundEffectSlider.value);
        }

        protected virtual void OnDisable()
        {
            saveManager.SetSoundEffectVolume(soundEffectSlider.value);

        }

        #endregion

        private void Initialize()
        {
            // We do not want to show the resolution and fullscreen settings if we are using WebGL
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                SetupResolutions();

                // Set the slider values to the saved values
                soundEffectSlider.value = saveManager.GetSoundEffectVolume();
                musicSlider.value = saveManager.GetMusicVolume();
            } 
            else
            {
                soundEffectSlider.value = 1;
                musicSlider.value = 1;

                // Disable the resolution and fullscreen groups if we are using WebGL
                resolutionGroup.SetActive(false);
                fullscreenToggleGroup.SetActive(false);
            }


            // Ensure the settings window is closed
            expandingScrollVertical.QuickDisableScroll();

            resumeButton?.onClick.AddListener(CloseSettings); 
            restartButton?.onClick.AddListener(ResetButtonPressed);
            exitButton?.onClick.AddListener(() => eventBus.Publish("QuitLevel"));
        }

        /// <summary>
        /// Setup resolution and fullscreen settings and listen to all related buttons
        /// </summary>
        private void SetupResolutions()
        {
            float aspectRatio = (Screen.currentResolution.width / Screen.currentResolution.height);

            nativeX = saveManager.NativeX;
            nativeY = saveManager.NativeY;

            supportedResolutions = new();

            if (nativeX >= 1080 && nativeY >= 1080)
            {
                //Debug.Log("Adding 1280x720");
                supportedResolutions.Add(new Resolution { width = 1280, height = 720 });
            }

            // If the screen is 1440p or higher, add 2560x1440
            if (nativeX >= 2560 && nativeY >= 1440)
            {
               // Debug.Log("Adding 2560x1440");
                supportedResolutions.Add(new Resolution { width = 2560, height = 1440 });
            }

            // If the screen is 4k or higher, add 3840x2160
            if (nativeX >= 3840 && nativeY >= 2160)
            {
                //Debug.Log("Adding 3840x2160");
                supportedResolutions.Add(new Resolution { width = 3840, height = 2160 });
            }

            // Set the resolution index based on the saved resolution
            for (int i = 0; i < supportedResolutions.Count; i++)
            {
                if (supportedResolutions[i].width == saveManager.ResolutionX && supportedResolutions[i].height == saveManager.ResolutionY)
                {
                    // Debug.Log($"Found resolution: {supportedResolutions[i].width}x{supportedResolutions[i].height}");
                    resolutionText.text = $"{supportedResolutions[i].width}x{supportedResolutions[i].height}";
                    resolution = i;
                    break;
                }
            }

            resolutionRight.onClick.AddListener(() =>
            {
                resolution += 1;
                CycleResolution();
            });

            resolutionLeft.onClick.AddListener(() =>
            {
                resolution -= 1;
                CycleResolution();
            });

            fullscreenButtonOn.onClick.AddListener(() => SetFullscreen(false));
            fullscreenButtonOff.onClick.AddListener(() => SetFullscreen(true));


            // Set the fullscreen state to the saved state
            if (saveManager.IsFullscreen)
            {
                SetFullscreen(true);
            }
            else
            {
                SetFullscreen(false);
            }

            // Get the current fullscreen state
            if (Screen.fullScreen)
            {
                fullscreenButtonOn.gameObject.SetActive(true);
                fullscreenButtonOff.gameObject.SetActive(false);
            }
            else
            {
                fullscreenButtonOn.gameObject.SetActive(false);
                fullscreenButtonOff.gameObject.SetActive(true);
            }

        }

        #region Window Management

        protected virtual void ResetButtonPressed()
        {
            eventBus.Publish("ResetLevel");
            eventBus.Publish("EnableMouseUsage");

            CloseSettings();
        }

        public virtual void OpenSettings()
        {
            if (settingsEnabled)
            {
                return;
            }

            settingsEnabled = true;
            expandingScrollVertical.EnableScroll();
        }

        public virtual void QuickOpenSettings()
        {
            if (settingsEnabled)
            {
                return;
            }

            settingsEnabled = true;
            expandingScrollVertical.QuickEnableScroll();
        }

        public virtual void CloseSettings()
        {
            if (!settingsEnabled)
            {
                return;
            }

            saveManager.SaveSettings();

            settingsEnabled = false;
            expandingScrollVertical.DisableScroll();
            eventBus.Publish("GameWindowClosed");
        }

        public virtual void QuickCloseSettings()
        {
            if (!settingsEnabled)
            {
                return;
            }

            settingsEnabled = false;
            expandingScrollVertical.QuickDisableScroll();
        }

        #endregion

        #region Resolution

        protected virtual void CycleResolution()
        {
            if (resolution < 0)
            {
                resolution = supportedResolutions.Count - 1;
            }
            else if (resolution > supportedResolutions.Count - 1)
            {
                resolution = 0;
            }

            Screen.SetResolution(supportedResolutions[resolution].width, supportedResolutions[resolution].height, isFullscreen);

            // For whatever hell of a reason, i need to fiddle with the rotation of the main camera when the resolution is at 1080p
            if (supportedResolutions[resolution].width == 1920 && supportedResolutions[resolution].height == 1080)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0.1f, 0.001f, 0);
            }
            else
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            resolutionText.text = $"{supportedResolutions[resolution].width}x{supportedResolutions[resolution].height}";

            saveManager.SetResolution(supportedResolutions[resolution].width, supportedResolutions[resolution].height);
        }

        #endregion

        #region Fullscreen

        protected virtual void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
            fullscreenButtonOn.gameObject.SetActive(fullscreen);
            fullscreenButtonOff.gameObject.SetActive(!fullscreen);

            saveManager.SetFullscreen(fullscreen);
        }

        #endregion
    }
}