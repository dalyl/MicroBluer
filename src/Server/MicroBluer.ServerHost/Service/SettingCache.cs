using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MicroBluer.ServerHost.Service
{
    public abstract class SettingCache<T> where T : class
    {

        static ObjectCache _cache = new MemoryCache("____AppSetting___");

        protected SettingCache()
        {
            try
            {
                AddCache();
            }
            catch
            {

            }
        }

        public abstract string Key { get; }

        public abstract string BasePath { get; set; }


        protected T Convert(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }


        protected abstract T GetDefault();

        protected string SettingPath
        {
            get
            {
                return $@"{ BasePath }\\AppSetting";
            }
        }

        protected string SettingFile
        {
            get
            {
                return $@"{SettingPath}\\{Key}";
            }
        }

        protected void CreateOrUpdateSettingFile(Func<T> GetValue)
        {
            var value = GetValue();
            var json = JsonConvert.SerializeObject(value);
            Directory.CreateDirectory(SettingPath);
            File.WriteAllText(SettingFile, json);
        }

        protected virtual void AddCache()
        {
            if (File.Exists(SettingFile) == false) CreateOrUpdateSettingFile(GetDefault);
            var policy = new CacheItemPolicy();
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { SettingFile }));
            var content = File.ReadAllText(SettingFile);
            if (string.IsNullOrEmpty(content)) CreateOrUpdateSettingFile(GetDefault);
            var cacheItem = new CacheItem(Key);
            cacheItem.Value = TryConvert(content);
            _cache.Add(cacheItem, policy);
        }

        T TryConvert(string content)
        {
            try
            {
                return Convert(content);
            }
            catch
            {
                return GetDefault();
            }
        }

        public T GetValue()
        {
            var value = _cache[Key];
            if (value == null) AddCache();
            return _cache[Key] as T;
        }
    }

}
