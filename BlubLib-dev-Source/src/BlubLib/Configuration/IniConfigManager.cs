//using System.Collections.Concurrent;
//using System.Collections.Generic;

//namespace BlubLib.Configuration
//{
//    public static class IniConfigManager
//    {
//        private static readonly ConcurrentDictionary<string, IniFile> _configs;

//        public static ICollection<IniFile> Configs
//        {
//            get { return _configs.Values; }
//        }

//        static IniConfigManager()
//        {
//            _configs = new ConcurrentDictionary<string, IniFile>();
//        }

//        public static IniFile Load(string key, string fileName)
//        {
//            return Load(key, fileName, new IniSerializer());
//        }
//        public static IniFile Load(string key, string fileName, IIniSerializer serializer)
//        {
//            var config = IniFile.Load(fileName, serializer);
//            return _configs.AddOrUpdate(key, config, (k, o) => config);
//        }

//        public static void Add(string key, IniFile config)
//        {
//            _configs.AddOrUpdate(key, config, (k, o) => config);
//        }
//        public static IniFile Get(string key)
//        {
//            return _configs[key];
//        }

//        public static void Save()
//        {
//            Save(new IniSerializer());
//        }
//        public static void Save(IIniSerializer serializer)
//        {
//            foreach (var config in Configs)
//                config.Save(serializer);
//        }

//        public static bool Remove(string key)
//        {
//            return _configs.Remove(key);
//        }
//        public static void Clear()
//        {
//            _configs.Clear();
//        }
//    }
//}
