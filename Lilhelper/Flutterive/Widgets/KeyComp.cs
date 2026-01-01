using System;
using System.Collections.Generic;

using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;

using UnityEngine;

namespace Lilhelper.Flutterive.Widgets {

    public interface IKeyApi {
        IDictionary<string, IWidget> KeyedWidgets { get; }
    }

    public class KeyApi : IKeyApi {
        public IDictionary<string, IWidget> KeyedWidgets { get; } = new Dictionary<string, IWidget>();
    }
}