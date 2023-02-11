using System;
using System.Collections.Generic;

namespace Feif
{
    public class PrefsData
    {
        public Dictionary<string, int> IntData = new Dictionary<string, int>();
        public Dictionary<string, string> StringData = new Dictionary<string, string>();
        public Dictionary<string, float> FloatData = new Dictionary<string, float>();

        public void DeleteAll()
        {
            IntData.Clear();
            StringData.Clear();
            FloatData.Clear();
        }

        public void DeleteKey(string key)
        {
            IntData.Remove(key);
            StringData.Remove(key);
            FloatData.Remove(key);
        }
    }
}