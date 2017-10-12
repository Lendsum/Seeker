using System;
using System.Collections.Generic;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    ///
    /// </summary>
    public class ThreadContextContainer : IDisposable
    {
        [ThreadStatic]
        private static Stack<Dictionary<string, object>> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextContainer" /> class.
        /// </summary>
        public ThreadContextContainer()
        {
            Stack.Push(new Dictionary<string, object>());
        }

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        /// <exception cref="LendsumException">The ThreadContextContainer is not initialized, we are not in a ThreadContextContainer enviroment</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public static Dictionary<string, object> CurrentContext
        {
            get
            {
                if (Stack.Count == 0) Stack.Push(new Dictionary<string, object>());
                return Stack.Peek();
            }
        }

        private static Stack<Dictionary<string, object>> Stack
        {
            get
            {
                if (stack == null) stack = new Stack<Dictionary<string, object>>();
                return stack;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stack.Pop();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}