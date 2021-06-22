using System;

using Plugin;

namespace HASCPlugin
{
    public class HASCPlugin : IPlugin
    {
        public string Name
        {
            get
            {
                return "HASCPlugin";
            }
        }

        public string Version
        {
            get
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                System.Version ver = asm.GetName().Version;
                return ver.ToString();
            }
        }

        public string Description
        {
            get
            {
                return "HASCプラグイン";
            }
        }

        public IPluginHost Owner
        {
            get; set;
        }

        public void Run()
        {

        }

        public void ExecuteTemplate(History history)
        {

        }
    }
}
