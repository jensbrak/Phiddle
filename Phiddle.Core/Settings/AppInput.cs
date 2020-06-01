using System;
using System.Collections.Generic;
using System.Linq;

namespace Phiddle.Core.Settings
{
    public class AppInput<T> : ISettings where T: Enum
    {
        // Hmm. Tried to use Dictionary<T, ActionId> but it fails serializing with ArgumentException("Key was already exist").
        // There's an issue registered about it that I believe apply to this use case, along with a pull request that is
        // not used. 
        //  
        // See https://github.com/neuecc/Utf8Json/issues/91 which points to
        // https://github.com/neuecc/Utf8Json/issues/128 which has a pull request
        // https://github.com/doominator42/Utf8Json/commit/f711bbc44829c57e32e80d0ffc99e469f4ab9cca that seem to solve the issue
        // The forked project with the fix has no package and I don't want to revert to repo instead of package so I'll just
        // keep this comment for future reference and store the keys as ushort instead. Not that important.
        public Dictionary<ushort, ActionId> InputMap { get; set; }

        public List<string> KeyList { get; set; }

        public AppInput()
        {
            // A little hack to get all available keys on the client platform easy to lookup
            // This means KeyList is not really used, which is a little... creative.
            var names = Enum.GetValues(typeof(T));
            var values = Enum.GetValues(typeof(T));

            KeyList = new List<string>(names.Length + 1)
            {
                $"Available keys (see enum {typeof(T)}):"
            };

            for (int i = 1; i < names.Length + 1; i++)
            {
                try
                {
                    var name = names.GetValue(i).ToString();
                    var value = Convert.ToUInt16(values.GetValue(i));
                    
                    KeyList.Add($"{name,-20}: {value}");
                }
                catch //(Exception ex)
                {
                    // Ignore this but continue if there are more valid ones
                    continue;
                }
            }
        }
    }
}
