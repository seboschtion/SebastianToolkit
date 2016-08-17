using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sebastian.Toolkit.MVVM.Container
{
    public class IoC
    {
        private static readonly Dictionary<string, object> Singletons = new Dictionary<string, object>();

        internal IoC() { }

        public static T GetInstance<T>(string key = null)
        {
            return (T)GetInstance(key ?? typeof(T).FullName);
        }

        private static object GetInstance(string name)
        {
            var entry = Singletons.FirstOrDefault(d => d.Key == name);
            if (entry.Value == null)
            {
                throw new KeyNotFoundException("Class is not instantiated.");
            }
            return entry.Value;
        }

        public TImplementation Singleton<TInterface, TImplementation>(params object[] parameters)
            where TImplementation : class, TInterface
        {
            var key = typeof(TInterface).FullName;
            return Singleton<TInterface, TImplementation>(key, parameters.Length > 0, parameters);
        }

        public TImplementation Singleton<TInterface, TImplementation>(string key)
            where TImplementation : class, TInterface
        {
            return Singleton<TInterface, TImplementation>(key, false, new object[0]);
        }

        public TImplementation Singleton<TInterface, TImplementation>(string key, params object[] parameters)
            where TImplementation : class, TInterface
        {
            return Singleton<TInterface, TImplementation>(key, parameters.Length > 0, parameters);
        }

        private TImplementation Singleton<TInterface, TImplementation>(string key, bool customParameters, params object[] parameters)
            where TImplementation : class, TInterface
        {
            var implementation = customParameters
                ? InitializeClassWithCustomParameters<TImplementation>(parameters)
                : InitializeClassWithDefaultParameters<TImplementation>();

            if (!Singletons.ContainsKey(key))
            {
                Singletons.Add(key, implementation);
                return implementation;
            }
            else
            {
                throw new Exception("This key is already registered.");
            }
        }

        private TImplementation InitializeClassWithDefaultParameters<TImplementation>()
        {
            var type = typeof(TImplementation);
            var parameterObjects = GetMethodsParameters(type.GetConstructors().First().GetParameters());
            return InitializeClass<TImplementation>(parameterObjects);
        }

        private TImplementation InitializeClassWithCustomParameters<TImplementation>(params object[] parameters)
        {
            return InitializeClass<TImplementation>(parameters);
        }

        private TImplementation InitializeClass<TImplementation>(object[] parameters)
        {
            var type = typeof(TImplementation);
            if (parameters.Length > 0)
            {
                return (TImplementation)Activator.CreateInstance(type, parameters);
            }
            else
            {
                return (TImplementation)Activator.CreateInstance(type);
            }
        }

        internal object[] GetMethodsParameters(ParameterInfo[] parameterInfos)
        {
            object[] parameters = new object[parameterInfos.Length];
            for(int i = 0; i < parameters.Length; i++)
            {
                var name = parameterInfos[i].ParameterType.FullName;
                parameters[i] = GetInstance(name);
            }
            return parameters;
        }
    }
}