using System;
using System.Text;
using UnityEngine;

namespace Lilhelper.Debugs {
    public static class DebugExts {
        public static Color32 TypeColor { get; set; } = new Color32(0x44, 0xF2, 0xD5, 0xFF);

        public static Color32 MethodColor { get; set; } = new Color32(0xDC, 0xDC, 0xAA, 0xFF);

        private static Color32 ExceptionColor { get; set; } = new Color32(0xFF, 0x44, 0x44, 0xFF);

        private static string ToHash(this Color32 color) {
            return $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
        }

        private static string TypeLog(this string txt) {
            return $"<color={TypeColor.ToHash()}>{txt}</color>";
        }

        private static string MethodLog(this string txt) {
            return $"<color={MethodColor.ToHash()}>{txt}</color>";
        }

        private static string ExceptionLog(this Exception ex) {
            if (ex is null) return "";
            return $"<b><color={ExceptionColor.ToHash()}>{ex.GetType()} : {ex.Message}</color></b>";
        }

        public static void F(this Type self, string methodName, params object[] args) {
            var msg = new StringBuilder($"{self.Name.TypeLog()}.{methodName.MethodLog()}(");
            if (args is not null && args.Length > 1) {
                string pad = new(' ', $"{self.Name}.{methodName}(".Length);
                foreach (object t in args) { msg.Append($"\n{pad}{t}"); }

                msg.Append(");");
                Debug.Log(msg.ToString());
                return;
            }

            if (args is not null && args.Length == 1) {
                msg.Append($"{args[0]});");
                Debug.Log(msg.ToString());
                return;
            }

            msg.Append(");");
            Debug.Log(msg.ToString());
        }

        public static void FuncThrow(this Type self, string methodName, Exception ex = null) {
            var msg = new StringBuilder($"{self.Name.TypeLog()}.{methodName.MethodLog()}(");
            if (ex is null) {
                msg.Append(");");
                Debug.Log(msg.ToString());
                return;
            }

            string pad = new(' ', $"{self.Name}.{methodName}(".Length);
            msg.AppendLine()
               .Append(pad)
               .Append(ExceptionLog(ex))
               .Append(");");
            Debug.Log(msg.ToString());
            throw ex;
        }
    }
}
