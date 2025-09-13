using CFGS.Core.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGS.Core.Runtime
{
    public class EnumInstance
    {
        public EnumDef EnumDef { get; }
        public string Name { get; }

        public EnumInstance(EnumDef enumDef, string name)
        {
            EnumDef = enumDef;
            Name = name;
        }

        public object Value => EnumDef.Members[Name]; // z. B. Index
    }

}
