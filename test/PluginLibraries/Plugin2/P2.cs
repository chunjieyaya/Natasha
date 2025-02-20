﻿using Newtonsoft.Json;
using PluginBase;

namespace Plugin2
{
    public class P2 : IPluginBase
    {
        public string PluginMethod1()
        {
            return $"Json:{typeof(JsonConvert).Assembly.GetName().Version};Dapper:{typeof(Dapper.SqlMapper).Assembly.GetName().Version};IPluginBase:{IPluginBase.Version};Self:{this.GetType().Assembly.GetName().Version}";
        }

        public string PluginMethod2(PluginModel pluginModel)
        {
            pluginModel = new PluginModel();
            return pluginModel.PluginName + pluginModel.PluginVersion;
        }
    }
}
