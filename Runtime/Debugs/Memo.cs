using System;
using UnityEngine;

namespace Lilhelper.Debugs {
    public class Memo : MonoBehaviour {
        [TextArea]
        [SerializeField] public string msg;

        private void OnEnable() {
            typeof(Memo).F(gameObject.name, msg);
        }
    }
}
