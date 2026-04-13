using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


namespace TowerBorn.SaveSystem
{
    public static class SavesManager
    {
        #region Properties and Fields
        public static bool saveAfterDelete = true;
        private static readonly string SavesDirectory;
        private static readonly string SettingsFilePath;
        private static readonly string ProgressFilePath;
        private static readonly string ResourcesFilePath;
        private static readonly string BuildingsSaveFilePath;

        private static SettingsSave _settingsSave;
        private static ResourcesSave _resourcesSave;
        private static ProgressSave _progressSave;

        public static SettingsSave SettingsSave => _settingsSave ??= new SettingsSave();

        public static ResourcesSave ResourcesSave => _resourcesSave ??= new ResourcesSave();

        public static ProgressSave ProgressSave => _progressSave ??= new ProgressSave();

        public static bool IsLoaded { get; private set; }

        public static event Action OnSavesLoaded;
        public static event Action OnFirstLaunch;

        public static event Action<SaveType> OnSaved;

        private static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        private static Formatting jsonFormatStyle = Formatting.Indented;

        private static string SaveSettingsPath = "SaveSettings";
        private static SaveSettings _saveSettings;
        #endregion

        #region Initialization

        static SavesManager()
        {
            SavesDirectory = Path.Combine(Application.persistentDataPath, "saves");
            SettingsFilePath = Path.Combine(SavesDirectory, "Settings.json");
            ProgressFilePath = Path.Combine(SavesDirectory, "Progress.json");
            ResourcesFilePath = Path.Combine(SavesDirectory, "Resources.json");
            BuildingsSaveFilePath = Path.Combine(SavesDirectory, "Buildings.json");

            if (!Directory.Exists(SavesDirectory))
            {
                Directory.CreateDirectory(SavesDirectory);
            }

            _settingsSave = new SettingsSave();
            _resourcesSave = new ResourcesSave();
            _progressSave = new ProgressSave();

            _saveSettings = Resources.Load<SaveSettings>(SaveSettingsPath);
        }

        #endregion

        #region Public Methods

        public static void LoadAll()
        {
            LoadSettings();
            LoadProgress();
            LoadResources();

            IsLoaded = true;
            OnSavesLoaded?.Invoke();
        }

        public static void SaveAll()
        {
            if (!saveAfterDelete) return;
#if UNITY_EDITOR
            if (IsSavesEnabled())
            {
                Debug.LogWarning($"[SavesManager] Saves disabled!");
                return;
            }
#endif
            SaveSettings();
            SaveProgress();
            SaveResources();
        }

        public static void DeleteAllSaves()
        {
            DeleteSettings();
            DeleteProgress();
            DeleteResources();

            _settingsSave = new SettingsSave();
            _resourcesSave = new ResourcesSave();
            _progressSave = new ProgressSave();

            IsLoaded = false;
            saveAfterDelete = false;
        }

        static bool IsSavesEnabled()
        {
            return _saveSettings != null && !_saveSettings.SaveSystemEnabled;
        }

#endregion

        #region Settings

        public static void LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                try
                {

                    string json = File.ReadAllText(SettingsFilePath);
                    _settingsSave = JsonConvert.DeserializeObject<SettingsSave>(json);
                    Debug.Log($"[Settings loaded from: {SettingsFilePath}]");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading settings: {e.Message}");
                    _settingsSave = new SettingsSave();
                }
            }
            else
            {
                Debug.Log("[Settings file not found, using defaults]");
                _settingsSave = new SettingsSave();
            }
        }

        public static void SaveSettings()
        {
#if UNITY_EDITOR
            if (IsSavesEnabled())
            {
                Debug.LogWarning($"[SavesManager] Saves disabled!");
                return;
            }
#endif
            try
            {
                string json = JsonConvert.SerializeObject(_settingsSave, jsonFormatStyle, settings);
                File.WriteAllText(SettingsFilePath, json);
                Debug.Log($"[Settings saved to: {SettingsFilePath}]");
                OnSaved?.Invoke(SaveType.Settings);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving settings: {e.Message}");
            }
        }

        public static void DeleteSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    File.Delete(SettingsFilePath);
                    Debug.Log($"[Settings file deleted: {SettingsFilePath}]");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deleting settings file: {e.Message}");
                }
            }
        }

        #endregion

        #region Progress

        public static void LoadProgress()
        {
            if (File.Exists(ProgressFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ProgressFilePath);
                    _progressSave = JsonConvert.DeserializeObject<ProgressSave>(json);
                    Debug.Log($"[Progress loaded from: {ProgressFilePath}]");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading progress: {e.Message}");
                    _progressSave = new ProgressSave();
                }
            }
            else
            {
                Debug.Log("[Progress file not found, using defaults]");
                _progressSave = new ProgressSave();
                OnFirstLaunch?.Invoke();
            }
        }

        /// <param name="ignoreTutorialBlock">Игнорировать ли блокировку сохранения во время туториала</param>
        public static void SaveProgress(bool ignoreTutorialBlock = false)
        {
#if UNITY_EDITOR
            if (IsSavesEnabled())
            {
                Debug.LogWarning($"[SavesManager] Saves disabled!");
                return;
            }
#endif
            try
            {
                _progressSave.lastExitDate = DateTime.Now;

                string json = JsonConvert.SerializeObject(_progressSave, jsonFormatStyle, settings);
                File.WriteAllText(ProgressFilePath, json);
                Debug.Log($"[Progress saved to: {ProgressFilePath}]");
                OnSaved?.Invoke(SaveType.Progress);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving progress: {e.Message}");
            }
        }

        public static void DeleteProgress()
        {
            if (File.Exists(ProgressFilePath))
            {
                try
                {
                    File.Delete(ProgressFilePath);
                    Debug.Log($"[Progress file deleted: {ProgressFilePath}]");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deleting progress file: {e.Message}");
                }
            }
        }

        public static bool IsFirstLaunch()
        {
            bool fileExists = File.Exists(ProgressFilePath);

            if (!fileExists || ProgressSave.isFirstLaunch)
            {
                ProgressSave.isFirstLaunch = false;
                SaveProgress();
                return true;
            }

            return false;
        }

        #endregion

        #region Resources

        public static void LoadResources()
        {
            if (File.Exists(ResourcesFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ResourcesFilePath);
                    _resourcesSave = JsonConvert.DeserializeObject<ResourcesSave>(json);
                    Debug.Log($"[Resources loaded from: {ResourcesFilePath}]");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading resources: {e.Message}");
                    _resourcesSave = new ResourcesSave();
                }
            }
            else
            {
                Debug.Log("[Resources file not found, using defaults]");
                _resourcesSave = new ResourcesSave();
            }
        }

        public static void SaveResources()
        {
#if UNITY_EDITOR
            if (IsSavesEnabled())
            {
                Debug.LogWarning($"[SavesManager] Saves disabled!");
                return;
            }
#endif
            try
            {
                string json = JsonConvert.SerializeObject(_resourcesSave, jsonFormatStyle, settings);
                File.WriteAllText(ResourcesFilePath, json);
                Debug.Log($"[Resources saved to: {ResourcesFilePath}]");
                OnSaved?.Invoke(SaveType.Resources);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving resources: {e.Message}");
            }
        }

        public static void DeleteResources()
        {
            if (File.Exists(ResourcesFilePath))
            {
                try
                {
                    File.Delete(ResourcesFilePath);
                    Debug.Log($"[Resources file deleted: {ResourcesFilePath}]");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deleting resources file: {e.Message}");
                }
            }
        }

        #endregion
    }

    public enum SaveType
    {
        Settings,
        Progress,
        Resources,
        Buildings
    }
}