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

        public void AddEntity(string EntityLogicalName)
        {

            if (!HasEntity(EntityLogicalName))
            {

                Entities.Add(new Entity()
                {
                    LogicalName = EntityLogicalName,
                    Fields = new List<Field>()
                });

            }

        }

        public void AddEntityField(string EntityLogicalName, string FieldLogicalName)
        {

            if (!HasEntity(EntityLogicalName))
            {
                AddEntity(EntityLogicalName);
            }

            if(!HasEntityField(EntityLogicalName, FieldLogicalName))
            {

                Entities.Where(x => x.LogicalName == EntityLogicalName).FirstOrDefault().Fields.Add(new Field()
                {
                    LogicalName = FieldLogicalName

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
        public List<Field> Fields;
    }

    public class Field
    {

        public string LogicalName;
    }
}
