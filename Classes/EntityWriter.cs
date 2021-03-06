﻿using Microsoft.Xrm.Sdk.Metadata;
using NArrange.Core.CodeElements;
using NArrange.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LandscapeInstitute.Dynamics.IEntityGenerator.Classes
{
    class EntityWriter
    {

        public string EntityLogicalName;
        public string EntitySchemaName;
        public string EntityDisplayName;
        public string OutputDirectory;
        public string Namespace;
        public string OptionSetNamespace;
        public string OutputFile;
        public string EntityPascalCase;
        public Boolean UsePartial;
        public string Usings;
        public string OutputSubDirectory;

        private string Classes;

        private string Body;

        public EntityWriter(string entityLogicalName, string entitySchemaName, string entityDisplayName, string outputDirectory, string classNamespace, string optionSetNamespace, Boolean usePartial, string usings, string entityOutputDir)
        {
            EntityLogicalName = entityLogicalName;
            Namespace = classNamespace;
            OptionSetNamespace = optionSetNamespace;
            EntitySchemaName = entitySchemaName;
            EntityDisplayName = entityDisplayName;
            EntityPascalCase = entityDisplayName.Replace(" ", String.Empty);

            OutputSubDirectory = entityOutputDir;

            UsePartial = usePartial;
            OutputDirectory = Path.Combine(outputDirectory, OutputSubDirectory);
            OutputFile = Path.Combine(OutputDirectory, $"{EntityPascalCase}.cs");
            Usings = usings.TrimEnd('\r', '\n') + Environment.NewLine; 
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

            using " + OptionSetNamespace + ";" + Environment.NewLine + 
            Usings + @"           
            using Newtonsoft.Json;
            using System;
            using System.ComponentModel.DataAnnotations;
            using System.ComponentModel.DataAnnotations.Schema;
            using System.Runtime.InteropServices;
            using System.Runtime.Serialization;

            namespace " + Namespace + @"{

                [DataContract(Name=""" + EntityLogicalName + @""")]
                public " + (UsePartial ? "partial" : "") + @" class " + EntityPascalCase + @" : IEntity 
                {
                    {0}
                }

            }

            ");
        }

        public void AddField(string fieldLogicalName, string fieldDisplayName, string dataType, Boolean optional = false) {

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            fieldDisplayName = rgx.Replace(fieldDisplayName, "");
            fieldDisplayName = fieldDisplayName.Replace(" ", "");

            Classes = Classes + (@"
   " + (optional ? "" : "[Required]") + @"
                    [JsonProperty(""" + fieldLogicalName + @""")]
                    public " + AttributeType(dataType, optional) + @" " + FieldName(fieldLogicalName, fieldDisplayName) + @" { get; set; }");
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
                LogWriter.LastFailed(Body);
                MessageBox.Show($"{ex.Message}", $"Error Generating {EntityLogicalName} Code", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }



        }

        public string AttributeType(string attributeType, Boolean optional)
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

                case "DateTime":
                    return $"DateTime{(optional ? "?" : "")}";

                case "Boolean":
                    return $"Boolean{(optional ? "?" : "")}";

                case "Virtual":
                    return "byte[]";

            }

            return attributeType;


        }

        public string FieldName(string fieldLogicalName, string fieldDisplayName)
        {

            if (fieldLogicalName == $"{EntityLogicalName.ToLower()}id")
                return "Id";

            if (fieldLogicalName == $"statuscode")
                return "StatusReason";

            if (fieldLogicalName == $"statecode")
                return "State";

            return fieldDisplayName;

        }


    }

}