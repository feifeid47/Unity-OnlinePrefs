using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Feif
{
    public static class OnlinePrefs
    {
        private static PrefsData data = new PrefsData();

        public static event Func<Task<byte[]>> OnLoadRequest;
        public static event Action<byte[]> OnSaveRequest;
        public static event Action<string> OnValueChanged;

        public static async Task LoadAsync()
        {
            var completionSource = new TaskCompletionSource<string>();
            var data = await OnLoadRequest?.Invoke();
            if (data == null) return;
            var json = Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(json)) return;
            var prefs = JsonConvert.DeserializeObject<PrefsData>(json);
            if (prefs == null) return;
            OnlinePrefs.data = prefs;
            foreach (var item in OnlinePrefs.data.IntData) PlayerPrefs.SetInt(item.Key, item.Value);
            foreach (var item in OnlinePrefs.data.FloatData) PlayerPrefs.SetFloat(item.Key, item.Value);
            foreach (var item in OnlinePrefs.data.StringData) PlayerPrefs.SetString(item.Key, item.Value);
        }

        public static void Save()
        {
            OnSaveRequest?.Invoke(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
        }

        public static void SetInt(string key, int value)
        {
            if (PlayerPrefs.GetInt(key) != value) OnValueChanged?.Invoke(key);
            PlayerPrefs.SetInt(key, value);
            data.IntData[key] = value;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            if (!data.IntData.TryGetValue(key, out var value))
            {
                value = PlayerPrefs.GetInt(key, defaultValue);
                SetInt(key, value);
            }
            return value;
        }

        public static void SetString(string key, string value)
        {
            if (PlayerPrefs.GetString(key) != value) OnValueChanged?.Invoke(key);

            PlayerPrefs.SetString(key, value);
            data.StringData[key] = value;
        }

        public static string GetString(string key, string defaultValue = "")
        {
            if (!data.StringData.TryGetValue(key, out var value))
            {
                value = PlayerPrefs.GetString(key, defaultValue);
                SetString(key, value);
            }
            return value;
        }

        public static void SetFloat(string key, float value)
        {
            if (PlayerPrefs.GetFloat(key) != value) OnValueChanged?.Invoke(key);

            PlayerPrefs.SetFloat(key, value);
            data.FloatData[key] = value;
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            if (!data.FloatData.TryGetValue(key, out var value))
            {
                value = PlayerPrefs.GetFloat(key, defaultValue);
                SetFloat(key, value);
            }
            return value;
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            data.DeleteAll();
            OnValueChanged?.Invoke(null);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            data.DeleteKey(key);
            OnValueChanged?.Invoke(key);
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}