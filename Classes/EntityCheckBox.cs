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

        public static readonly DependencyProperty SchemaNameProperty =
              DependencyProperty.Register("SchemaName", typeof(string), typeof(EntityCheckBox));

        public string SchemaName
        {
            get { return (string)GetValue(SchemaNameProperty); }
            set { SetValue(SchemaNameProperty, value); }
        }

        public static readonly DependencyProperty LogicalNameProperty =
              DependencyProperty.Register("LogicalName", typeof(string), typeof(EntityCheckBox));

        public string LogicalName
        {
            get { return (string)GetValue(LogicalNameProperty); }
            set { SetValue(LogicalNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty =
              DependencyProperty.Register("DisplayName", typeof(string), typeof(EntityCheckBox));

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

    }


   
}
