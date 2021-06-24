using System;
using System.Windows.Forms;

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
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Text File (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    new DataRowCreator(Owner, dialog.FileName);
                }
            }
        }

        public void ExecuteTemplate(History history)
        {

        }
    }
}
