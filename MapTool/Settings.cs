using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapTool
{
    internal class Settings : ApplicationSettingsBase
    {
        [CompilerGenerated]
        [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
        internal static Settings defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"C:\Program Files (x86)")]
        public string defaultPath
        {
            get
            {
                return (string)this["defaultPath"];
            }
            set
            {
                this["defaultPath"] = value;
            }
        }
    }
}
