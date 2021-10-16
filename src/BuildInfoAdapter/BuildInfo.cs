using System;
using System.Reflection;

namespace BuildInfoAdapter
{
    public static class BuildInfo
    {
        private static readonly Assembly a = Assembly.GetCallingAssembly();
        private static readonly AssemblyName an = a.GetName();
        private static readonly Version v = an.Version;

        public static readonly string Name = an.Name;
        public static readonly string FullName = an.FullName;
        public static readonly string Version = v.ToString();

        [Obsolete("BuildDate is deprecated, please use Version instead.")]
        public static readonly string BuildDate = v.Build + "." + v.Revision;

        // currently unimplemented - provided for compatibility
        public static readonly string BuildComputer = "";
        public static readonly string BuildUser = "";
    }
}
