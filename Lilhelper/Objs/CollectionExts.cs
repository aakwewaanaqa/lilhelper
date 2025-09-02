using System;
using System.Collections.Generic;
using System.Linq;

namespace Lilhelper.Objs {
    public static class CollectionExts {
        public static IDictionary<int, T> ToIndexDict<T>(this T[] self) {
            var dict = new Dictionary<int, T>();
            for (int i = 0; i < self.Length; i++) {
                dict.Add(i, self[i]);
            }

            return dict;
        }

        public static IEnumerable<T> OutIndexDict<T>(this IEnumerable<T> self, out IDictionary<int, T> dict) {
            dict = new Dictionary<int, T>();
            int index = 0;
            foreach (var item in self) {
                dict.Add(index++, item);
            }

            return self;
        }

        public static IEnumerable<T> OutArray<T>(this IEnumerable<T> self, out T[] arr) {
            arr = self.ToArray();
            return self;
        }
    }
}
