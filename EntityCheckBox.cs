using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LandscapeInstitute.Dynamics.IEntityGenerator
{
    public class EntityCheckBox : CheckBox
    {
        public static readonly DependencyProperty ParentEntityProperty =
              DependencyProperty.Register("ParentEntity", typeof(string), typeof(EntityCheckBox));

        public string ParentEntity
        {
            get { return (string)GetValue(ParentEntityProperty); }
            set { SetValue(ParentEntityProperty, value); }
        }
    }


   
}
