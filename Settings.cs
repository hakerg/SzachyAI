using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzachyAI
{
    enum HintMode { DrawOnScreen, WindowLabel, TextToSpeech };

    enum MouseMode { Draging, Clicking };

    static class Settings
    {
        public static bool enableDebugMode = false;
        public static bool showBorder = true;
        public static HintMode hintMode = HintMode.DrawOnScreen;
        public static MouseMode mouseMode = MouseMode.Draging;
        public static int eventTime = 100; // ms
        public static int findingTime = 3; // s
    }
}
