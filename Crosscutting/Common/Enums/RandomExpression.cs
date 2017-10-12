namespace Lendsum.Crosscutting.Common.Enums
{
    /// <summary>
    /// Enum to represent variables in expressions used to generate randomly fillable strings
    /// </summary>
    public enum RandomExpressionType
    {
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A random numeric character
        /// </summary>
        Numeric = 1,

        /// <summary>
        /// A random alphanumeric character
        /// </summary>
        Alphanumeric = 2,

        /// <summary>
        /// A random alphabetic lower case character
        /// </summary>
        AlphabeticLowerCase = 3,

        /// <summary>
        /// A random alphabetic upper case character
        /// </summary>
        AlphabeticUpperCase = 4,

        /// <summary>
        /// The email of the user
        /// </summary>
        Email = 5
    }
}