using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzachyAI
{
    static class Settings
    {
        private static bool enableDebugMode = false;
        public static bool EnableDebugMode
        {
            get => enableDebugMode;
            set => enableDebugMode = value;
        }
    }
}
