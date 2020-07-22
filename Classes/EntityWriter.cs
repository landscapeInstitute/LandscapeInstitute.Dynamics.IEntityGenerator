﻿using Microsoft.Xrm.Sdk.Metadata;
using NArrange.Core.CodeElements;
using NArrange.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LandscapeInstitute.Dynamics.IEntityGenerator.Classes
{
    class EntityWriter
    {

        public string EntityLogicalName;
        public string OutputDirectory;
        public string NameSpace;
        public string OutputFile;
        public string EntityNiceName;

        private string Classes;

        private string Body;

        public EntityWriter(string entityLogicalName, string outputDirectory, string classNamespace)
        {
            EntityLogicalName = entityLogicalName;
            NameSpace = classNamespace;
            EntityNiceName = EntityLogicalName.Split('_').LastOrDefault();
            EntityNiceName = char.ToUpper(EntityNiceName[0]) + EntityNiceName.Substring(1);

            OutputDirectory = Path.Combine(outputDirectory, "Entites");
            OutputFile = Path.Combine(OutputDirectory, $"{EntityNiceName}.cs");

            Directory.CreateDirectory(OutputDirectory);

            WriteBody();

        }

        public void WriteToFile(string content)
        {
            File.AppendAllText(OutputFile, string.Format("{0}{1}", content, Environment.NewLine));
        }

        private string Content()
        {
            return File.ReadAllText(OutputFile);
        }

        private void WriteBody()
        {

            Body = Body + (@"
            //------------------------------------------------------------------------------
            // <auto-generated>
            //     This code was generated by entity generation.
            //
            //     Changes to this file may cause incorrect behavior and will be lost if
            //     the code is regenerated.
            // </auto-generated>
            //------------------------------------------------------------------------------

            using Felinesoft.Framework.Core;
            using Felinesoft.Framework.CoreInterfaces;
            using Felinesoft.Framework.Ecommerce.Models;
            using System;

            namespace " + NameSpace + @"{

                [EntityName(""" + EntityLogicalName + @""")]
                public class " + EntityNiceName + @" : IEntity 
                {
                    {0}
                }

            }

            ");
        }

        public void AddField(string fieldLogicalName, string fieldDisplayName, string dataType, Boolean optional = false) {
      
            Classes = Classes + (@"
   " + (optional ? "[Optional]" : "") + @"
                    [FieldName(""" + fieldLogicalName + @""")]
                    public " + AttributeType(dataType) + @" " + FieldLogicalName(fieldLogicalName) + @" { get; set; }");
        }

        public void Generate()
        {

            try
            {
                Body = Body.Replace("{0}", Classes);

                StringReader reader = new StringReader(Body);
                CSharpParser parser = new CSharpParser();
                StringWriter writer = new StringWriter();
                CSharpWriter formatter = new CSharpWriter();
                ReadOnlyCollection<ICodeElement> elements = parser.Parse(reader);

                formatter.Write(elements, writer);

                Body = writer.ToString();

                WriteToFile(Body);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"{ex.Message}", "Error Generating Entity, Parse Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }



        }

        public string AttributeType(string attributeType)
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

                case "Lookup":
                    return "EntityReference";

                case "Customer":
                    return "CustomerEntityReference";

                case "Status":
                    return $"{EntityLogicalName}Statuscode";


            }

            return attributeType;


        }

        public string FieldLogicalName(string attributeLogicalName)
        {

            if (attributeLogicalName == $"{EntityLogicalName.ToLower()}id")
                return "Id";

            if (attributeLogicalName == $"statuscode")
                return "StatusReason";

            if (attributeLogicalName == $"statecode")
                return "Status";

            attributeLogicalName = attributeLogicalName.Split('_').LastOrDefault();

            return char.ToUpper(attributeLogicalName[0]) + attributeLogicalName.Substring(1);

        }


    }

}