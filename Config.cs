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
            Entities = new List<string>();
        }

        public List<string> Entities { get; set; }

        public String Url { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String OutputDirectory { get; set; }


        public String Namespace { get; set; }

    }
}
