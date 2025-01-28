using Core;
using Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Management
{
    public class GameSettingsWindow : MonoBehaviour
    {
        public static GameSettingsWindow Instance;

        [SerializeField] protected Button exitButton, restartButton, resumeButton;

        [SerializeField] protected ExpandingScrollVectical expandingScrollVertical;

        [Header("Resolution Elements")]
        [SerializeField] protected Button resolutionRight, resolutionLeft;
        [SerializeField] protected TMPro.TextMeshProUGUI resolutionText;

        [Header("Fullscreen Elements")]
        [SerializeField] protected Button fullscreenButtonOn, fullscreenButtonOff;

        [Header("Sound Effect Elements")]
        [SerializeField] protected Slider soundEffectSlider;
        [SerializeField] protected Slider musicSlider;

        protected EventBus eventBus;
        protected SaveData saveData;
        protected SoundEffectManager SoundEffectManager;

        protected bool settingsEnabled = false;
        protected int resolution = 0;

        protected int nativeX, nativeY;

        protected bool isFullscreen => Screen.fullScreen;

        protected List<Resolution> supportedResolutions;

        #region Unity Functions

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one GameSettingsWindow instance in scene!");
                Destroy(this);
                return;
            }
        }

        protected virtual void Start()
        {
            // Give the native screen resolution to the resolution
           // Debug.Log($"Native resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height}");


            saveData = SaveData.Instance;

            float aspectRatio = (Screen.currentResolution.width / Screen.currentResolution.height);
            //  Debug.Log($"Aspect ratio: {aspectRatio}");

            nativeX = saveData.NativeX;
            nativeY = saveData.NativeY;

            supportedResolutions = new();

            if (nativeX >= 1080 && nativeY >= 1080)
            {
                Debug.Log("Adding 1920x1080");
                supportedResolutions.Add(new Resolution { width = 1920, height = 1080 });
            }

            // If the screen is 1440p or higher, add 2560x1440
            if (nativeX >= 2560 && nativeY >= 1440)
            {
                Debug.Log("Adding 2560x1440");
                supportedResolutions.Add(new Resolution { width = 2560, height = 1440 });
            }

            // If the screen is 4k or higher, add 3840x2160
            if (nativeX >= 3840 && nativeY >= 2160)
            {
                Debug.Log("Adding 3840x2160");
                supportedResolutions.Add(new Resolution { width = 3840, height = 2160 });
            }

            eventBus = EventBus.Instance;
            saveData = SaveData.Instance;
            SoundEffectManager = SoundEffectManager.Instance;

            // Set the resolution index based on the saved resolution
            for (int i = 0; i < supportedResolutions.Count; i++)
            {
                if (supportedResolutions[i].width == saveData.settingsData.resolutionX && supportedResolutions[i].height == saveData.settingsData.resolutionY)
                {
                   // Debug.Log($"Found resolution: {supportedResolutions[i].width}x{supportedResolutions[i].height}");
                    resolutionText.text = $"{supportedResolutions[i].width}x{supportedResolutions[i].height}";
                    resolution = i;
                    break;
                }
            }

            // Set the slider values to the saved values
            soundEffectSlider.value = saveData.settingsData.soundEffectVolume;
            musicSlider.value = saveData.settingsData.musicVolume;

            //Debug.Log($"Setting sound effect volume to: {saveData.settingsData.soundEffectVolume}");

            expandingScrollVertical.DisableScroll();

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

            // Load the saved graphics data

            // Set the fullscreen state to the saved state
            if (saveData.settingsData.isFullscreen)
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

            resumeButton.onClick.AddListener(CloseSettings);
            restartButton.onClick.AddListener(ResetButtonPressed);
            exitButton.onClick.AddListener(() => eventBus.Publish("QuitLevel"));
        }

        protected virtual void Update()
        {
            // Check for changes to the sound effect volume
            if (SoundEffectManager.SoundEffectVolume != soundEffectSlider.value)
            {
                SoundEffectManager.SoundEffectVolume = soundEffectSlider.value;
            }

            //if (saveData.settingsData.soundEffectVolume != musicSlider.value)
            //{
            //    saveData.settingsData.musicVolume = musicSlider.value;
            //}
        }

        // When the scene is changed, or the game is closed, save the settings data
        protected virtual void OnApplicationQuit()
        {
            saveData.settingsData.soundEffectVolume = soundEffectSlider.value;
            saveData.SaveSettingsDataToJson();
        }

        protected virtual void OnDisable()
        {
            saveData.settingsData.soundEffectVolume = soundEffectSlider.value;
            saveData.SaveSettingsDataToJson();
        }

        #endregion

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

            saveData.SaveSettingsDataToJson();

            settingsEnabled = false;
            expandingScrollVertical.FadeOutScroll();
            eventBus.Publish("GameWindowClosed");
        }

        public virtual void QuickCloseSettings()
        {
            if (!settingsEnabled)
            {
                return;
            }

            settingsEnabled = false;
            expandingScrollVertical.DisableScroll();
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

            saveData.settingsData.resolutionX = supportedResolutions[resolution].width;
            saveData.settingsData.resolutionY = supportedResolutions[resolution].height;
            saveData.SaveSettingsDataToJson();
        }

        #endregion

        #region Fullscreen

        protected virtual void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
            fullscreenButtonOn.gameObject.SetActive(fullscreen);
            fullscreenButtonOff.gameObject.SetActive(!fullscreen);

            saveData.settingsData.isFullscreen = fullscreen;
            saveData.SaveSettingsDataToJson();
        }

        #endregion
    }
}