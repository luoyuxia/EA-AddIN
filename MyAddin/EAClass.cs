﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;

namespace MyAddin
{
    class EAClass
    {
        private string name;
        private string stereoType;
        private List<EAAttribute> attributes = new List<EAAttribute>();
        private EA.Element element;
        private string id;
        private string realizeClassId = null;
        private List<string> measurementAxisList = new List<string>();

        public EAClass() { }
        public EAClass(string name)
        {
            this.name = name;
        }
        public EAClass(string name, string stereoType):this(name)
        {
            this.stereoType = stereoType;
        }

        public EAClass(string name ,string stereoType, string id):this(name, stereoType)
        {
            this.id = id;
        }

        public EAClass(string name, string stereoType, string id, string realizeId):this(name, stereoType, id)
        {
            this.realizeClassId = realizeId;
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<string> MeasurementAxisList
        {
            get { return measurementAxisList; }
            set { measurementAxisList = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public string RealizeClassID
        {
            get { return realizeClassId; }
            set { realizeClassId = value; }
        }


        public string StereoType
        {
            get { return stereoType; }
            set { stereoType = value; }
        }

        public List<EAAttribute> Attributes
        {
            get { return attributes; }
        }

        public Element Element { get => element; set => element = value; }

        public void addAttribute(EAAttribute attribute)
        {
            this.attributes.Add(attribute);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void removeAttribute(EAAttribute attribute)
        {
            this.attributes.Remove(attribute);
        }

        public override string ToString()
        {
            return String.Format("{{id = {0}, name = {1}, steroType = {2}, attributes = [{3}], realizeClassId = {4}}}", id, name, stereoType, String.Join(",", attributes), realizeClassId);
        }
    }
}
