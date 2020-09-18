using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeInstitute.Dynamics.IEntityGenerator
{
    public class Config
    {

        public Config()
        {
            Entities = new List<Entity>();
        }

        public List<Entity> Entities { get; set; }

        public String Url { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String OutputDirectory { get; set; }
        public String EntityNamespace { get; set; }
        public string OptionsetNamespace { get; set; }
        public string EntityOutputDir { get; set; }
        public string OptionsetOutputDir { get; set; }
        public Boolean UsePartial { get; set; }
        public string AdditionalUsings { get; set; }

        public Boolean HasEntity(string EntityLogicalName)
        {
            if(Entities.Where(x => x.LogicalName == EntityLogicalName).Any()){
                return true;
            }

            return false;
        }

        public Boolean HasEntityField(string EntityLogicalName, string FieldLogicalName)
        {

            if (!HasEntity(EntityLogicalName)) return false;

            if (Entities.Where(x => x.LogicalName == EntityLogicalName).Any())
            {
                if(Entities.Where(x => x.LogicalName == EntityLogicalName).FirstOrDefault().Fields.Where(x => x.LogicalName == FieldLogicalName).Any())
                {
                    return true;
                }
            }

            return false;
        }

        public void AddEntity(string LogicalName, string SchemaName, string DisplayName )
        {

            if (!HasEntity(LogicalName))
            {

                Entities.Add(new Entity()
                {
                    LogicalName = LogicalName,
                    SchemaName = SchemaName,
                    DisplayName = DisplayName,
                    Fields = new List<Field>()
                });

            }

        }

        public void AddEntityField(string ParentLogicalName, string ParentSchemaName, string ParentDisplayName, string LogicalName, string SchemaName, string DisplayName)
        {

            if (!HasEntity(ParentLogicalName))
            {
                AddEntity(ParentLogicalName, ParentSchemaName, ParentDisplayName);
            }

            if(!HasEntityField(ParentLogicalName, LogicalName))
            {

                Entities.Where(x => x.LogicalName == ParentLogicalName).FirstOrDefault().Fields.Add(new Field()
                {
                    LogicalName = LogicalName,
                    SchemaName = SchemaName,
                    DisplayName = DisplayName

                });

            }

        }

        public void RemoveEntity(string EntityLogicalName)
        {

            if (HasEntity(EntityLogicalName))
            {

                Entities.Remove(Entities.Where(x => x.LogicalName == EntityLogicalName).FirstOrDefault());

            }

        }

        public void RemoveEntityField(string EntityLogicalName, string FieldLogicalName)
        {

            if (HasEntityField(EntityLogicalName, FieldLogicalName))
            {
                var EntityField = Entities.Where(x => x.LogicalName == EntityLogicalName).FirstOrDefault().Fields.Where(x => x.LogicalName == FieldLogicalName).FirstOrDefault();
                Entities.Where(x => x.LogicalName == EntityLogicalName).FirstOrDefault().Fields.Remove(EntityField);

            }
        }


    }

    public class Entity
    {
        public string LogicalName;
        public string SchemaName;
        public string DisplayName;
        public List<Field> Fields;
    }

    public class Field
    {

        public string LogicalName;
        public string SchemaName;
        public string DisplayName;
    }
}
