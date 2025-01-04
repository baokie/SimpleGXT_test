using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 简单关系图_测试_
{
    public class AppConfigManager
    {
        private readonly string _configFilePath;
        public AppConfigManager(string configFilePath)
        {
            _configFilePath = configFilePath;
            // 创建配置文件夹
            if (!Directory.Exists(Path.GetDirectoryName(_configFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));
            }
        }

        public string GetString(string key, string defaultValue)
        {
            try
            {
                var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _configFilePath };
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                {
                    return config.AppSettings.Settings[key]?.Value ?? defaultValue;
                }
                else
                {
                    config.AppSettings.Settings.Add(key, defaultValue);
                    config.Save(ConfigurationSaveMode.Modified);
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"配置文件读取异常: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return defaultValue; // 出现异常，返回默认值
            }
        }

        public void SaveString(string key, string value)
        {
            try
            {
                var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _configFilePath };
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    config.AppSettings.Settings[key].Value = value;
                }
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置文件异常: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
