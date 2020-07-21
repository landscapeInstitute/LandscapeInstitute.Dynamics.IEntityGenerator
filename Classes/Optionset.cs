using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeInstitute.Dynamics.IEntityGenerator.Classes
{
    class Optionset
    {

        public string LogicalName;

        public Dictionary<int, string> Options;

        public Optionset()
        {

            Options = new Dictionary<int, string>();
        }

        public void Add(int value, string label)
        {
            Options.Add(value, label);

        }

    }
}
