using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfdScriptUtil
{
    static class TemplateStrings
    {
        public const string start = "/* SCRIPT STARTS HERE - COPY BELOW INTO THE SCRIPT WINDOW */";
        public const string end = "/* SCRIPT ENDS HERE - COPY ABOVE INTO THE SCRIPT WINDOW */";
        public const string startClass = "/* CLASS STARTS HERE - COPY BELOW INTO THE SCRIPT WINDOW */";
        public const string endClass = "/* CLASS ENDS HERE - COPY ABOVE INTO THE SCRIPT WINDOW */";

        public static string GetCsprojString()
        {
            string steampath = @"C:\Program Files (x86)\Steam";
            string sysvar = Environment.GetEnvironmentVariable("steampath", EnvironmentVariableTarget.User);
            if (sysvar == null)
            {
                Console.WriteLine("Failed to get steam path, using default.");
            }
            else
            {
                steampath = sysvar;
            }
            return @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net4.6.2</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""SFD.GameScriptInterface"">
      <HintPath>" + steampath + @"\steamapps\common\Superfighters Deluxe\SFD.GameScriptInterface.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
";

        }

        public const string programStart = @"using SFDGameScriptInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace SFDScript
{
    public partial class GameScript : GameScriptInterface
    {
        /// <summary>
        /// Placeholder constructor that's not to be included in the ScriptWindow!
        /// </summary>
        public GameScript() : base(null) { }

        /* SCRIPT STARTS HERE - COPY BELOW INTO THE SCRIPT WINDOW */";
        public const string programEnd = @"        /* SCRIPT ENDS HERE - COPY ABOVE INTO THE SCRIPT WINDOW */
    }
}

";
        public const string classStart = @"using SFDGameScriptInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SFDScript
{
    public partial class GameScript : GameScriptInterface
    {
        /* CLASS STARTS HERE - COPY BELOW INTO THE SCRIPT WINDOW */";

        public const string classEnd = @"        /* CLASS ENDS HERE - COPY ABOVE INTO THE SCRIPT WINDOW */
    }
}";
    }
}
