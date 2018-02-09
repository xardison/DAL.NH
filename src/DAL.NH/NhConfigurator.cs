using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Util;

namespace DAL.NH
{
    public interface INhConfigurator
    {
        Configuration GetNewConfiguration();
    }

    public class NhConfigurator : INhConfigurator
    {
        public const string HibernateCfgFile = "hibernate.cfg.xml";

        #region private fields
        private static readonly IDictionary<string, string> NhCfgProperties = new Dictionary<string, string>();
        private static readonly IList<string> MappingAssemblys = new List<string>();
        private static string _hibernateCfgFilePath;
        private static object _lockObject = new object();
        #endregion

        public static void SetProperty(string propertyName, string value)
        {
            NhCfgProperties.Add(propertyName, value);
        }
        public static void ResetPropertys()
        {
            NhCfgProperties.Clear();
        }
        public static void SetNhibernateCfgFilePath(string nhibernateCfgFilePath)
        {
            _hibernateCfgFilePath = nhibernateCfgFilePath;
        }

        public static void AddAssembly(IEnumerable<string> assemblyNames)
        {
            assemblyNames.ForEach(AddAssembly);
        }
        public static void AddAssembly(IEnumerable<Type> types)
        {
            types.ForEach(AddAssembly);
        }
        public static void AddAssembly(Type type)
        {
            AddAssembly(type.Assembly.FullName);
        }
        public static void AddAssembly(string assemblyName)
        {
            lock (_lockObject)
            {
                if (assemblyName != null && !MappingAssemblys.Contains(assemblyName))
                {
                    MappingAssemblys.Add(assemblyName);
                }
            }
        }
        public static void AddEntryAssembly()
        {
            AddAssembly(Assembly.GetEntryAssembly().FullName);
        }
        public static void ResetAssemblyMappingList()
        {
            lock (_lockObject)
            {
                MappingAssemblys.Clear();
            }
        }

        public Configuration GetNewConfiguration()
        {
            var configuration = new Configuration();

            ConfigureWithCfgFile(configuration);
            ConfigureWithUserProps(configuration);

            InnerAddAssembly(configuration);
            InnerSetConnectionString(configuration);

            return configuration;
        }

        // private methods ======================================================================
        private void ConfigureWithCfgFile(Configuration configuration)
        {
            var defaultCfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HibernateCfgFile);

            if (File.Exists(_hibernateCfgFilePath))
            {
                configuration.Configure(_hibernateCfgFilePath);
            }
            else if (File.Exists(defaultCfgPath))
            {
                configuration.Configure(defaultCfgPath);
            }
        }
        private void ConfigureWithUserProps(Configuration configuration)
        {
            NhCfgProperties.ForEach(prop => configuration.SetProperty(prop.Key, prop.Value));
        }
        private void InnerAddAssembly(Configuration configuration)
        {
            lock (_lockObject)
            {
                MappingAssemblys.Distinct().ForEach(assemblyName => { configuration.AddAssembly(assemblyName); });
            }
        }
        private void InnerSetConnectionString(Configuration configuration)
        {
            var cs = ConnectionStringManager.GetConnectionString();
            configuration.SetProperty("connection.connection_string", cs);
        }
    }
}