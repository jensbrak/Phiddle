using System;
using System.Collections.Generic;

namespace Phiddle.Core.Settings
{
    public class AppInput : ISettings
    {
        public Dictionary<ushort, ActionId> Keys { get; set; }
    }
}
