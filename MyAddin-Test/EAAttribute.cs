using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAddin_Test
{
    class EAAttribute
    {
        private string name;

        private string type;

        private bool isEAClass = false;

        public bool IsEAClass
        {
            get { return isEAClass; }
            set { isEAClass = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public EAAttribute() { }

        public EAAttribute(string name)
        {
            this.name = name;
        }

        public EAAttribute(string name, string type): this(name)
        {
            this.type = type;
        }

        public EAAttribute(string name, string type, bool isEAClass): this(name, type)
        {
            this.isEAClass = isEAClass;
        }

        public override string ToString()
        {
            return String.Format("{{name = {0}, type = {1}, isClass = {2}}}", name, type, isEAClass);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
