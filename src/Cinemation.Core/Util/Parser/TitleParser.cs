using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cinemation.Core.Util.Parser
{
    public class TitleParser
    {
        public static bool Is3D(string Info)
        {
            Regex ThreeD = new Regex(@"\.3D\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return ThreeD.IsMatch(Info);
        }

        public static string GetYear(string Info)
        {
            Regex Year = new Regex(@"\.([1-2][0-9]{3})\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var Result = Year.Match(Info);

            if(Result.Success)
            {
                return Result.Groups[1].Value;
            }

            return null;
        }

        public static string GetResolution(string Info)
        {
            return null;
        }

        public static string GetReleaseGroup(string Info)
        {
            return null;
        }

        public static string GetSource(string Info)
        {
            return null;
        }

        public static string GetVideoCodec(string Info)
        {
            return null;
        }

        public static string GetAudioCodec(string Info)
        {
            return null;
        }
    }
}
