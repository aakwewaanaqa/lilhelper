using Lilhelper.Debugs;
using NUnit.Framework;

namespace Tests {
    public class DebugTests {
        [Test]
        public void Test() {
            typeof(DebugTests).F(nameof(Test), "1", 2);
        }
    }
}
