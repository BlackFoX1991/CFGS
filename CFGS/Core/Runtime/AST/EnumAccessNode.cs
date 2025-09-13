namespace CFGS.Core.Runtime.AST
{
    /// <summary>
    /// Defines the <see cref="EnumAccessNode" />
    /// </summary>
    public class EnumAccessNode : Node
    {
        /// <summary>
        /// Defines the EnumName
        /// </summary>
        public readonly string EnumName;

        /// <summary>
        /// Defines the MemberName
        /// </summary>
        public readonly string MemberName;

        /// <summary>
        /// Gets or sets the Column
        /// </summary>
        public override int Column { get; set; }

        /// <summary>
        /// Gets or sets the Line
        /// </summary>
        public override int Line { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumAccessNode"/> class.
        /// </summary>
        /// <param name="enumName">The enumName<see cref="string"/></param>
        /// <param name="memberName">The memberName<see cref="string"/></param>
        /// <param name="column">The column<see cref="int"/></param>
        /// <param name="line">The line<see cref="int"/></param>
        public EnumAccessNode(string enumName, string memberName, int column, int line)
        {
            EnumName = enumName;
            MemberName = memberName;
            Column = column;
            Line = line;
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString() => $"{EnumName}.{MemberName}";
    }
}
