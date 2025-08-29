using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lilhelper.Data.Attributes;
using UnityEngine;

namespace Lilhelper.Data {
    public class CsvParser {
        private readonly ParseArgs args;

        public CsvParser(ParseArgs args) {
            this.args = args;
        }

        public IEnumerable<T> Parse<T>(string src) where T : new() {
            src = src.Replace("\r\n", "\n");

            string[] rows = src.Split(args.rowDelimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] keys = rows[0].Split(args.columnDelimiter);
            var nvfs =
                typeof(T)
                   .GetFields()
                   .Select(f => new Nvf {
                        name  = f.GetCustomAttribute<Column>()?.Name,
                        field = f,
                    })
                   .Where(n => n.name is not null)
                   .ToDictionary(n => n.name, n => n.field);

            for (int y = 1; y < rows.Length; y++) {
                string[] columns  = rows[y].Split(args.columnDelimiter);
                var      instance = new T();
                for (int x = 0; x < keys.Length; x++) {
                    string key = keys[x];
                    if (nvfs.TryGetValue(key, out var field)) {
                        try {
                            string column = columns[x].Trim();
                            object value  = Convert.ChangeType(column, field.FieldType);
                            field.SetValue(instance, value);
                        }
                        catch (Exception e) {
                            Debug.Log($"{y}:{x}:{e}");
                            throw;
                        }
                    }
                }

                yield return instance;
            }

            yield break;
        }
    }
}
