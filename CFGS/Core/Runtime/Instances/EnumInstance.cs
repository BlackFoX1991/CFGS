using CFGS.Core.Analytics;

namespace CFGS.Core.Runtime.Instances
{
    /// <summary>
    /// Defines the <see cref="EnumInstance" />
    /// </summary>
    public class EnumInstance
    {
        /// <summary>
        /// Gets the EnumDef
        /// </summary>
        public EnumDef EnumDef { get; }

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumInstance"/> class.
        /// </summary>
        /// <param name="enumDef">The enumDef<see cref="EnumDef"/></param>
        /// <param name="name">The name<see cref="string"/></param>
        public EnumInstance(EnumDef enumDef, string name)
        {
            EnumDef = enumDef;
            Name = name;
        }

        /// <summary>
        /// Gets the Value
        /// </summary>
        public object Value => EnumDef.Members[Name];// z. B. Index
    }

}
