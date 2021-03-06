﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;

namespace MyAddin
{
    class EAAttribute
    {
        private string name;
        private EA.Attribute attribute;

        private string type;

        private bool isEAClass = false;
        private EAAttributeValue lowerValue;
        private EAAttributeValue upperValue;
        private string attributeType;

        private string id;

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

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public EAAttributeValue UpperValue { get => upperValue; set => upperValue = value; }
        public EAAttributeValue LowerValue { get => lowerValue; set => lowerValue = value; }
        public string AttributeType { get => attributeType; set => attributeType = value; }
        public EA.Attribute Attribute { get => attribute; set => attribute = value; }

        public EAAttribute() { }

        public EAAttribute(string name)
        {
            this.name = name;
        }

        public EAAttribute(string name, string type): this(name)
        {
            this.type = type;
        }

        public EAAttribute(string name, string type, string id) : this(name)
        {
            this.id = id;
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
