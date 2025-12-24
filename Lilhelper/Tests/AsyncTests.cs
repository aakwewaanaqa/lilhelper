using System.Collections;
using Lilhelper.Async;
using UnityEngine;
using UnityEngine.TestTools;

namespace Lilhelper.Tests
{
    public class AsyncTests
    {
        [UnityTest]
        public IEnumerator TestEnumerating()
        {
            using var ch = new Channel<int>(new Ctx());
            yield break;
        }
    }
}