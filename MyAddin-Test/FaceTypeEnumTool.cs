using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAddin_Test
{
    class FaceTypeEnumTool
    {
        public string getFaceElementType(String faceType)
        {
            string type = "unknow";
            FaceTypeEnum faceTypeEnum = parseFaceType(faceType);
            switch(faceTypeEnum)
            {
                case FaceTypeEnum.PortableComponent:
                case FaceTypeEnum.PlatformSpecificComponent:
                case FaceTypeEnum.MessagePort:
                    type = "uop";
                    break;
            }
            return type;
        }

        public string getPackageChildrenElementTypePrefix(string packageType)
        {
            string type = "unkown";
            FaceTypeEnum faceTypeEnum = parseFaceType(packageType);
            switch (faceTypeEnum)
            {
                case FaceTypeEnum.ConceptualDataModel:
                    type = "conceptual";
                    break;
                case FaceTypeEnum.LogicalDataModel:
                    type = "logical";
                    break;
                case FaceTypeEnum.PlatformDataModel:
                    type = "platform";
                    break;
                case FaceTypeEnum.UoPModel:
                    type = "uop";
                    break;
            }
            return type;
        }

        public string getPacakgeTag(string packageType)
        {
            string type = "unkown";
            FaceTypeEnum faceTypeEnum = parseFaceType(packageType);
            switch(faceTypeEnum)
            {
                case FaceTypeEnum.ConceptualDataModel:
                    type = "cdm";
                    break;
                case FaceTypeEnum.LogicalDataModel:
                    type = "ldm";
                    break;
                case FaceTypeEnum.PlatformDataModel:
                    type = "pdm";
                    break;
                case FaceTypeEnum.UoPModel:
                    type = "uopModel";
                    break;
            }
            return type;
        }

        public string getElementTag(string elementType)
        {
            string type = "element";
            FaceTypeEnum faceTypeEnum = parseFaceType(elementType);
            switch(faceTypeEnum)
            {
                case FaceTypeEnum.MessagePort:
                    type = "port";
                    break;
            }
            return type;
        }

        public string getElementAttributeTag(string attributeType)
        {
            string type = "composition";
            FaceTypeEnum faceTypeEnum = parseFaceType(attributeType);
            switch(faceTypeEnum)
            {
                case FaceTypeEnum.CharacteristicProjection:
                    type = "characteristic";
                    break;
                case FaceTypeEnum.MessagePort:
                    type = "port";
                    break;
            }
            return type;
        }

        public bool isChildrenElemnt(string pacakgeType)
        {
            FaceTypeEnum faceTypeEnum = parseFaceType(pacakgeType);
            return faceTypeEnum.Equals(FaceTypeEnum.MessagePort);
        }

        public FaceTypeEnum parseFaceType(string faceType)
        {
            try
            {
                return (FaceTypeEnum)Enum.Parse(typeof(FaceTypeEnum), faceType);
            }
            catch(Exception)
            {
                return FaceTypeEnum.UNKNOW;
            }
        }
    }
}
