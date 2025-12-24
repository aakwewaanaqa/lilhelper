using System.Collections;
using Lilhelper.GetIt;
using UnityEngine;

namespace Lilhelper.Async
{
    /// <summary>
    /// 沒有什麼特別的意義，只是用來異步跑協成
    /// </summary>
    public class R : MonoBehaviour
    {
        private static R i => GetItComp.I.Value.Get<R>();

        public static IEnumerator Run(IEnumerator routine)
        {
            yield return i.StartCoroutine(routine);
        }
    }
}