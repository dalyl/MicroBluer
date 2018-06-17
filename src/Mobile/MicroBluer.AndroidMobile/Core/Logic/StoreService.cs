namespace MicroBluer.AndroidMobile.Logic
{
    using MicroBluer.AndroidMobile.Stroage;
    using Newtonsoft.Json;

    public abstract class StoreService<T>
    {
        protected abstract string SharedFileName { get; }

        public SharedStore Shared { get; }

        public StoreService()
        {
            Shared = new SharedStore(SharedFileName);
        }

        protected T Get(string key)
        {
            var value = Shared.GetValue(key, string.Empty);
            if (string.IsNullOrEmpty(value)) return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        protected bool Save(string key, T model)
        {
            if (model == null) return false;
            var value = JsonConvert.SerializeObject(model);
            Shared.PutValue(key, value);
            return true;
        }

        protected bool Delete(string key)
        {
            Shared.Remove(key);
            return true;
        }

    }

}