using System;

namespace Lilhelper.Data.Attributes {
    public class Column : Attribute {
        public string Name;

        public Column(string name) {
            Name = name;
        }
    }
}
