﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApsimNG.Cloud.Azure {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class AzureSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static AzureSettings defaultInstance = ((AzureSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AzureSettings())));
        
        public static AzureSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OutputDir {
            get {
                return ((string)(this["OutputDir"]));
            }
            set {
                this["OutputDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LicenceFilePath {
            get {
                return ((string)(this["LicenceFilePath"]));
            }
            set {
                this["LicenceFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("16")]
        public string NumCPUCores {
            get {
                return ((string)(this["NumCPUCores"]));
            }
            set {
                this["NumCPUCores"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string APSIMDirectory {
            get {
                return ((string)(this["APSIMDirectory"]));
            }
            set {
                this["APSIMDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string APSIMZipFile {
            get {
                return ((string)(this["APSIMZipFile"]));
            }
            set {
                this["APSIMZipFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string APSIMVersion {
            get {
                return ((string)(this["APSIMVersion"]));
            }
            set {
                this["APSIMVersion"] = value;
            }
        }
    }
}
