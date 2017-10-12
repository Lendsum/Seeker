using Microsoft.Extensions.Configuration;
using System;
using Xunit;
using System.IO;

namespace Core.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
                
        }
    }

    public class ConfigurationExample
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
    }
}
