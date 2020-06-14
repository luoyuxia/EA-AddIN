using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAddin
{
    class EAAttributeValue
    {
        private string type;
        private string value;
        private string id;

        public EAAttributeValue(string type, string id, string value)
        {
            this.type = type;
            this.id = id;
            this.value = value;
        }

        public EAAttributeValue(string value)
        {
            this.value = value;
        }

        public string Id { get => id; set => id = value; }
        public string Value { get => value; set => this.value = value; }
        public string Type { get => type; set => type = value; }
    }
}
