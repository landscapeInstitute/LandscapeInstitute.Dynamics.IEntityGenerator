using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LandscapeInstitute.Dynamics.IEntityGenerator
{
    public static class EntityToolset
    {

        public static Boolean IgnoreAttributeType(string attributeType)
        {
            switch (attributeType)
            {

                case "Money":
                    return true;


            }

            return false;

        }

        public static string ConvertAttributeType(string attributeType)
        {

            switch (attributeType)
            {

                case "Money":
                    return "Decimal";

                case "Memo":
                    return "String";

                case "Uniqueidentifier":
                    return "Guid";

                case "State":
                    return "DefaultState";


            }

            return attributeType;


        }

        public static string ConvertLogicalNameToDisplayName(string entityLogicalName, string attributeLogicalName)
        {

            if (attributeLogicalName == $"{entityLogicalName.ToLower()}id")
                return "Id";

            if (attributeLogicalName == $"statuscode")
                return "StatusReason";

            if (attributeLogicalName == $"statecode")
                return "Status";

            return char.ToUpper(attributeLogicalName[0]) + attributeLogicalName.Substring(1);

        }



    }
}
