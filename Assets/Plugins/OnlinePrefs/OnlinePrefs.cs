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

        public static async Task Initialize()
        {
            if (PlayerPrefs.GetInt("OnlinePrefs-IsSync", 0) == 0)
            {
                var completionSource = new TaskCompletionSource<string>();
                var raw = await OnLoadRequest?.Invoke();
                if (raw == null) return;
                var json = Encoding.UTF8.GetString(raw);
                if (string.IsNullOrEmpty(json)) return;
                PlayerPrefs.SetString("OnlinePrefs", json);
                var prefs = JsonConvert.DeserializeObject<PrefsData>(json);
                if (prefs == null) return;
                data = prefs;
                PlayerPrefs.SetInt("OnlinePrefs-IsSync", 1);
            }
            else
            {
                var json = PlayerPrefs.GetString("OnlinePrefs-Data", null);
                if (string.IsNullOrEmpty(json)) return;
                var prefs = JsonConvert.DeserializeObject<PrefsData>(json);
                if (prefs == null) return;
                data = prefs;
            }
        }

        public static void Save()
        {
            OnSaveRequest?.Invoke(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            PlayerPrefs.SetString("OnlinePrefs-Data", JsonConvert.SerializeObject(data));
        }

        public static void SetInt(string key, int value)
        {
            if (data.IntData.ContainsKey(key) && data.IntData[key] == value) return;
            data.IntData[key] = value;
            PlayerPrefs.SetString("OnlinePrefs-Data", JsonConvert.SerializeObject(data));
            OnValueChanged?.Invoke(key);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            int value;
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetInt(key, defaultValue);
                SetInt(key, value);
                PlayerPrefs.DeleteKey(key);
                return value;
            }
            if (data.IntData.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static void SetString(string key, string value)
        {
            if (data.StringData.ContainsKey(key) && data.StringData[key] == value) return;
            data.StringData[key] = value;
            PlayerPrefs.SetString("OnlinePrefs-Data", JsonConvert.SerializeObject(data));
            OnValueChanged?.Invoke(key);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            string value;
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetString(key, defaultValue);
                SetString(key, value);
                PlayerPrefs.DeleteKey(key);
                return value;
            }
            if (data.StringData.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static void SetFloat(string key, float value)
        {
            if (data.FloatData.ContainsKey(key) && data.FloatData[key] == value) return;
            data.FloatData[key] = value;
            PlayerPrefs.SetString("OnlinePrefs-Data", JsonConvert.SerializeObject(data));
            OnValueChanged?.Invoke(key);
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            float value;
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key, defaultValue);
                SetFloat(key, value);
                PlayerPrefs.DeleteKey(key);
                return value;
            }
            if (data.FloatData.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static void DeleteAll()
        {
            data.DeleteAll();
            PlayerPrefs.SetString("OnlinePrefs-Data", JsonConvert.SerializeObject(data));
            OnValueChanged?.Invoke(null);
        }

        public static void DeleteKey(string key)
        {
            data.DeleteKey(key);
            PlayerPrefs.SetString("OnlinePrefs-Data", JsonConvert.SerializeObject(data));
            OnValueChanged?.Invoke(key);
        }

        public static bool HasKey(string key)
        {
            return data.IntData.ContainsKey(key) || data.FloatData.ContainsKey(key) || data.StringData.ContainsKey(key);
        }
    }
}