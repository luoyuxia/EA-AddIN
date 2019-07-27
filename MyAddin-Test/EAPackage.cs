using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAddin_Test
{
    class EAPackage
    {
        private string stereoType;
        private string name;
        private List<EAPackage> packages = new List<EAPackage>();
        private List<EAClass> classes = new List<EAClass>();

        public EAPackage() { }

        public EAPackage(string name) { this.name = name; }

        public EAPackage(string name, string stereoType) { this.name = name; this.stereoType = stereoType; }

        public string StereoType
        {
            get { return stereoType; }
            set { stereoType = value;}
        }
        public string Name
        {
            get { return name;  }
            set { name = value;  }
        }

        public List<EAPackage> EAPackages
        {
            get { return packages;  }
            set { packages = value; }
        }

        public List<EAClass> EAClass
        {
            get { return classes; }
            set { classes = value; }
        }

        public void addPackage(EAPackage package)
        {
            this.packages.Add(package);
        }

        public void removePackage(EAPackage package)
        {
            this.packages.Remove(package);
        }

        public void addClass(EAClass eaClass)
        {
            this.classes.Add(eaClass);
        }

        public void removeClass(EAClass eaClass)
        {
            this.classes.Remove(eaClass);
        }

        public override string ToString()
        {
            return String.Format("{{name = {0}, stereoType = {1}, packages = [{2}], classes =  [{3}]}}", name, stereoType,
                String.Join(",", packages), String.Join(",", classes));
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
