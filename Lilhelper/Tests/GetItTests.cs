using System.Collections;

using Lilhelper.Async;
using Lilhelper.GetIt;
using Lilhelper.Objs;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Lilhelper.Tests {
    public class GetItTests {

        private GetItComp Gi => GetItComp.I.Value;

        [UnityTest]
        public IEnumerator TestR() {
            Gi.Register(() => {
                new GameObject(nameof(R)).AddCompOut(out R r).Keep();
                return r;
            });

            R.Run(WaitFor5Seconds());
            var r = GameObject.FindFirstObjectByType<R>();
            Assert.That(r.DoExists(), Is.True);
            yield return new WaitForSeconds(6);
            yield break;

            static IEnumerator WaitFor5Seconds() {
                yield return new WaitForSeconds(5);
            }
        }

        [UnityTest]
        public IEnumerator TestExactType() {
            Gi.Register(() => {
                new GameObject(nameof(BaseComp)).AddCompOut(out BaseComp c).Keep();
                return c;
            });
            var comp = Gi.Get<BaseComp>();
            Assert.That(comp.DoExists(), Is.True);
            yield return new WaitForSeconds(5);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInheritedType() {
            yield break;
        }
    }
}