﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace LandscapeInstitute.Dynamics.IEntityGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IOrganizationService _organizationService;
        private string _appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private string _configFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "config.json");
        private Config _config = new Config();

        #region Methods

        public MainWindow()
        {
            InitializeComponent();

            LoadConfig();
            ClearOutputDirectory();

            _organizationService = GetOrganiszationService();

            SetStatus("Ready...");

        }

        public IOrganizationService GetOrganiszationService()
        {

            if (Username.Text == string.Empty)
                Username.Background = System.Windows.Media.Brushes.LightSalmon;

            if (Password.Text == string.Empty)
                Password.Background = System.Windows.Media.Brushes.LightSalmon;

            if (Url.Text == string.Empty)
                Url.Background = System.Windows.Media.Brushes.LightSalmon;

            if (Username.Text == string.Empty || Password.Text == string.Empty || Url.Text == string.Empty)
            {
                SetStatus("Set Dynamics365 Connection Details");
                return null;
            }
            else
            {
                Username.Background = System.Windows.Media.Brushes.White;
                Password.Background = System.Windows.Media.Brushes.White;
                Url.Background = System.Windows.Media.Brushes.White;
            }

            SetStatus("Connecting to Dynamics...");
            var connectionString = $"AuthType = Office365; Url = {Url.Text}; Username = {Username.Text}; Password = {Password.Text}";
            CrmServiceClient conn = new CrmServiceClient(connectionString);

            IOrganizationService service;
            service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

            return service;

        }

        public void LoadEntities()
        {

            SaveConfig();

            MenuItem root = new MenuItem() { Value = "Entities" };

            if (_organizationService == null) return;

            try
            {

                this.Dispatcher.Invoke(() =>
                {
                    GetEntities_Button.IsEnabled = false;
                });

                SetStatus("Requesting Entities List...");

                RetrieveAllEntitiesRequest retrieveEntityRequest = new RetrieveAllEntitiesRequest
                {
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true
                };

                RetrieveAllEntitiesResponse retrieveAllEntitiesResponse = (RetrieveAllEntitiesResponse)_organizationService.Execute(retrieveEntityRequest);
                var entities = retrieveAllEntitiesResponse.EntityMetadata.OrderBy(x => x.LogicalName);

                this.Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 0;
                    ProgressBar.Maximum = entities.Count();
                });

                foreach (EntityMetadata entity in entities)
                {

                    SetStatus($"Loading {entity.SchemaName}");

                    this.Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Value = ProgressBar.Value + 1;
                    });


                    MenuItem entityMenuItem = new MenuItem() {
                        Value = entity.LogicalName,
                        Checked = _config.HasEntity(entity.LogicalName),
                        ParentEntity = null
                    };

                    foreach (AttributeMetadata field in entity.Attributes.OrderBy(x => x.LogicalName))
                    {
                        if (!string.IsNullOrWhiteSpace(field.LogicalName))
                        {
                            MenuItem fieldMenuItem = new MenuItem() {
                                Value = field.LogicalName,
                                Checked = _config.HasEntityField(entity.LogicalName, field.LogicalName),
                                ParentEntity = entity.LogicalName
                            };
                            entityMenuItem.Items.Add(fieldMenuItem);
                        }

                    }

                    root.Items.Add(entityMenuItem);

                }

                this.Dispatcher.Invoke(() =>
                {
                    TreeView.Items.Clear();
                    TreeView.Items.Add(root);
                });


                this.Dispatcher.Invoke(() =>
                {
                    GetEntities_Button.IsEnabled = true;
                });

                SetStatus("Ready...");

            }
            catch (Exception ex)
            {

                MessageBox.Show($"{ex.Message}", "Unable to Download Entities", MessageBoxButton.OK, MessageBoxImage.Error);

                this.Dispatcher.Invoke(() =>
                {
                    GetEntities_Button.IsEnabled = true;
                });



            }


        }

        public void WriteToEntityFile(string logicalName, string content)
        {
            var filePath = Path.Combine(_config.OutputDirectory, $"{logicalName}.cs");
            File.AppendAllText(filePath, string.Format("{0}{1}", content, Environment.NewLine));
        }

        public void ClearOutputDirectory()
        {

            if (Directory.Exists(_config.OutputDirectory))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(_config.OutputDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }



        }

        public void CreateCSharpCode()
        {

            SaveConfig();

            if (_organizationService == null) return;

            try
            {

                if (!Directory.Exists(_config.OutputDirectory))
                {
                    Directory.CreateDirectory(_config.OutputDirectory);
                }

                this.Dispatcher.Invoke(() =>
                {
                    GetEntities_Button.IsEnabled = false;
                });


                this.Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 0;
                    ProgressBar.Maximum = _config.Entities.Count();
                });

                foreach (Entity entity in _config.Entities)
                {


                    WriteToEntityFile(entity.LogicalName, @"

                    using Felinesoft.Framework.Core;
                    using Felinesoft.Framework.CoreInterfaces;
                    using Felinesoft.Framework.Ecommerce.Models;
                    using System;

                    namespace " + _config.Namespace + @"
                    {
                        [EntityName(""" + entity.LogicalName + @""")]
                        public class " + entity.LogicalName + @" : IEntity
                        {
                    ");

                    this.Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Value = ProgressBar.Value + 1;
                    });

                    SetStatus($"Requesting Entities Details for {entity.LogicalName}...");

                    RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
                    {
                        LogicalName = entity.LogicalName,
                        EntityFilters = EntityFilters.All,
                    };

                    RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)_organizationService.Execute(retrieveEntityRequest);


                    foreach (AttributeMetadata field in retrieveEntityResponse.EntityMetadata.Attributes)
                    {

                        if (EntityToolset.IgnoreAttributeType(field.AttributeType.ToString()) == false)
                        {
                            WriteToEntityFile(entity.LogicalName, @"
                            [FieldName(""" + field.LogicalName + @""")]
                            public " + EntityToolset.ConvertAttributeType(field.AttributeType.ToString()) + @" " + EntityToolset.ConvertLogicalNameToDisplayName(field.EntityLogicalName, field.LogicalName) + @" { get; set; }
                            ");
                        }

                    }


                    WriteToEntityFile(entity.LogicalName, @"
                        }
                    }");

                }

                SetStatus("Ready...");

            }
            catch (Exception ex)
            {

                MessageBox.Show($"{ex.Message}", "Unable to Download Entities", MessageBoxButton.OK, MessageBoxImage.Error);

                this.Dispatcher.Invoke(() =>
                {
                    GetEntities_Button.IsEnabled = true;
                });



            }



        }

        public void SaveConfig()
        {
            this.Dispatcher.Invoke(() =>
            {
                _config.Username = Username.Text;
                _config.Password = Password.Text;
                _config.Url = Url.Text;
                _config.OutputDirectory = OutputDir.Text;
                _config.Namespace = Namespace.Text;

                if (File.Exists(_configFile)) File.Delete(_configFile);

                string json = JsonConvert.SerializeObject(_config);
                System.IO.File.WriteAllText(_configFile, json);
            });

        }

        public void LoadConfig()
        {

            if (File.Exists(_configFile))
            {
                using (StreamReader r = new StreamReader(_configFile))
                {
                    string json = r.ReadToEnd();
                    _config = JsonConvert.DeserializeObject<Config>(json);
                }
            }


            Username.Text = _config.Username;
            Password.Text = _config.Password;
            Url.Text = _config.Url;
            OutputDir.Text = _config.OutputDirectory;
            Namespace.Text = _config.Namespace;



        }
        public void SetStatus(string statusText)
        {

            this.Dispatcher.Invoke(() =>
            {
                Status.Content = statusText;
            });

        }

        #endregion

        #region Classes

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Value { get; set; }

            public Boolean Checked { get; set; }

            public String ParentEntity { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        #endregion

        #region Events

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EntityCheckBox checkbox = (EntityCheckBox)sender;

            if (string.IsNullOrWhiteSpace(checkbox.ParentEntity))
            {

                _config.AddEntity(checkbox.Content.ToString());

            }
            else
            {

                _config.AddEntityField(checkbox.ParentEntity, checkbox.Content.ToString());
            }

            SaveConfig();

        }

        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            EntityCheckBox checkbox = (EntityCheckBox)sender;

            if (string.IsNullOrWhiteSpace(checkbox.ParentEntity))
            {

                _config.RemoveEntity(checkbox.Content.ToString());

            }
            else
            {

                _config.RemoveEntityField(checkbox.ParentEntity, checkbox.Content.ToString());
            }

            SaveConfig();

        }

        private void SaveConfig_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void BrowseOutput_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.ShowDialog();
            OutputDir.Text = dialog.SelectedPath;

        }

        private void SaveConfig_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void GetEntities_Button_Click(object sender, RoutedEventArgs e)
        {
            new Thread(LoadEntities).Start();
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            new Thread(CreateCSharpCode).Start();
        }

        #endregion


    }
}
