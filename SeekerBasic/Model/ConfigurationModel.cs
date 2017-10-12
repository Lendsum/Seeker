using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Model
{
    public class ConfigurationModel
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
        public IEnumerable<string> MuchosStrings { get; set; }
    }
}
