using System;

namespace Lendsum.Crosscutting.Common.Providers
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Lendsum.Crosscutting.Common.IRandomGenerator" />
    public class RandomGenerator : IRandomGenerator
    {
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomGenerator"/> class.
        /// </summary>
        public RandomGenerator()
        {
            this.random = new Random();
        }

        /// <summary>
        /// Nexts the specified maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Next(int maxValue)
        {
            return this.random.Next(maxValue);
        }
    }
}