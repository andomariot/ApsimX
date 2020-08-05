﻿using APSIM.Shared.Utilities;
using ApsimNG.Cloud;
using Models.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserInterface.Views;
using static UserInterface.Views.UpgradeView;

namespace UserInterface.Presenters
{
    /// <summary>
    /// Presenter which allows user to send a job to run on a cloud platform.
    /// </summary>
    /// <remarks>
    /// Currently, the only supported/implemented cloud platform is azure.
    /// If this class is extended (ie if you remove the sealed modifier)
    /// please remember to update the IDisposable implementation.
    /// </remarks>
    public sealed class RunOnCloudPresenter : IPresenter
    {
        /// <summary>The view.</summary>
        private ViewBase view;

        /// <summary>The explorer presenter.</summary>
        private ExplorerPresenter presenter;

        /// <summary>The name of the job edit box.</summary>
        private EditView nameOfJobEdit;

        /// <summary>The combobox for number of cpus.</summary>
        private DropDownView numberCPUCombobox;

        private DropDownView apsimTypeToRunCombobox;

        /// <summary>The label with text 'Directory'.</summary>
        private LabelView directoryLabel;

        /// <summary>The directory edit box.</summary>
        private EditView directoryEdit;

        /// <summary>The browse button.</summary>
        private ButtonView browseButton;

        /// <summary>The version label.</summary>
        private LabelView versionLabel;

        /// <summary>The version combobox.</summary>
        private DropDownView versionCombobox;

        /// <summary>The submit button.</summary>
        private ButtonView submitButton;

        /// <summary>The status label.</summary>
        private LabelView statusLabel;

        /// <summary>The model which we want to run on Azure.</summary>
        private IModel modelToRun;

        /// <summary>Cloud interface responsible for job submission.</summary>
        private ICloudInterface cloudInterface;

        /// <summary>Allows job submission to be cancelled.</summary>
        private CancellationTokenSource cancellation;

        /// <summary>Default constructor.</summary>
        public RunOnCloudPresenter()
        {
            cloudInterface = new AzureInterface();
            cancellation = new CancellationTokenSource();
        }

        /// <summary>Attaches this presenter to a view.</summary>
        /// <param name="model"></param>
        /// <param name="viewBase"></param>
        /// <param name="parentPresenter"></param>
        public void Attach(object model, object viewBase, ExplorerPresenter parentPresenter)
        {
            presenter = parentPresenter;
            view = (ViewBase)viewBase;
            modelToRun = (IModel)model;

            nameOfJobEdit = view.GetControl<EditView>("nameOfJobEdit");
            numberCPUCombobox = view.GetControl<DropDownView>("numberCPUCombobox");
            apsimTypeToRunCombobox = view.GetControl<DropDownView>("apsimTypeToRunCombobox");
            directoryLabel = view.GetControl<LabelView>("directoryLabel");
            directoryEdit = view.GetControl<EditView>("directoryEdit");
            browseButton = view.GetControl<ButtonView>("browseButton");
            versionLabel = view.GetControl<LabelView>("versionLabel");
            versionCombobox = view.GetControl<DropDownView>("versionCombobox");
            submitButton = view.GetControl<ButtonView>("submitButton");
            statusLabel = view.GetControl<LabelView>("statusLabel");

            nameOfJobEdit.Text = modelToRun.Name + DateTime.Now.ToString("yyyy-MM-dd HH.mm");
            numberCPUCombobox.Values = new string[] { "16", "32", "48", "64", "80", "96", "112", "128", "256" };
            numberCPUCombobox.SelectedValue = ApsimNG.Cloud.Azure.AzureSettings.Default.NumCPUCores;

            apsimTypeToRunCombobox.Values = new string[] { "A released version", "A directory", "A zip file" };
            if (!string.IsNullOrEmpty(ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMVersion))
                apsimTypeToRunCombobox.SelectedValue = "A released version";
            else if (!string.IsNullOrEmpty(ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMDirectory))
                apsimTypeToRunCombobox.SelectedValue = "A directory";
            else if (!string.IsNullOrEmpty(ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMZipFile))
                apsimTypeToRunCombobox.SelectedValue = "A zip file";
            else
                apsimTypeToRunCombobox.SelectedValue = "A released version";

            SetupWidgets();

            if (!string.IsNullOrEmpty(ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMVersion))
                versionCombobox.SelectedValue = ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMVersion;
            else if (!string.IsNullOrEmpty(ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMDirectory))
                directoryEdit.Text = ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMDirectory;
            else if (!string.IsNullOrEmpty(ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMZipFile))
                directoryEdit.Text = ApsimNG.Cloud.Azure.AzureSettings.Default.APSIMZipFile;

            apsimTypeToRunCombobox.Changed += OnVersionComboboxChanged;
            browseButton.Clicked += OnBrowseButtonClicked;
            submitButton.Clicked += OnSubmitJobClicked;
        }

        /// <summary>This instance has been detached.</summary>
        public void Detach()
        {
            ApsimNG.Cloud.Azure.AzureSettings.Default.Save();
            cancellation.Dispose();
            apsimTypeToRunCombobox.Changed -= OnVersionComboboxChanged;
            browseButton.Clicked -= OnBrowseButtonClicked;
            submitButton.Clicked -= OnSubmitJobClicked;
        }

        /// <summary>User has changed the selection in the version combobox.</summary>
        private void OnVersionComboboxChanged(object sender, EventArgs e)
        {
            SetupWidgets();
        }

        /// <summary>User has clicked the browse button.</summary>
        private void OnBrowseButtonClicked(object sender, EventArgs e)
        {
            string newValue;
            if (apsimTypeToRunCombobox.SelectedValue == "A directory")
                newValue = ViewBase.AskUserForFileName("Select the APSIM directory", 
                                                       Utility.FileDialog.FileActionType.SelectFolder, 
                                                       directoryEdit.Text);
            else
                newValue = ViewBase.AskUserForFileName("Please select a zipped file", 
                                                       Utility.FileDialog.FileActionType.Open, 
                                                       "Zip file (*.zip) | *.zip");
            if (!string.IsNullOrEmpty(newValue))
                directoryEdit.Text = newValue;
        }

        /// <summary>User has clicked the submit button.</summary>
        private async void OnSubmitJobClicked(object sender, EventArgs e)
        {
            try
            {
                if (submitButton.Text == "Submit")
                {
                    submitButton.Text = "Cancel submit";
                    await SubmitJob(this, EventArgs.Empty);
                }
                else
                {
                    submitButton.Text = "Submit";
                    cancellation.Cancel();
                }

            }
            catch (Exception err)
            {
                presenter.MainPresenter.ShowError(err);
            }
        }

        /// <summary>Cancels submission of a job.</summary>
        public void CancelJobSubmission(object sender, EventArgs args)
        {
            cancellation.Cancel();
        }

        /// <summary>/hide the directory and version controls based on the value in the version dropdown.</summary>
        private void SetupWidgets()
        {
            versionLabel.Visible = apsimTypeToRunCombobox.SelectedValue == "A released version";
            versionCombobox.Visible = versionLabel.Visible;
            directoryLabel.Visible = !versionLabel.Visible;
            directoryEdit.Visible = !versionLabel.Visible;
            browseButton.Visible = !versionLabel.Visible;

            if (versionLabel.Visible)
            {
                // Populate the version drop down.
                if (versionCombobox.Values.Length == 0)
                {
                    presenter.MainPresenter.ShowWaitCursor(true);
                    try
                    {
                        var upgrades = WebUtilities.CallRESTService<Upgrade[]>("https://apsimdev.apsim.info/APSIM.Builds.Service/Builds.svc/GetUpgradesSinceIssue?issueID=-1");
                        var upgradeNames = upgrades.Select(upgrade =>
                        {
                            var name = upgrade.ReleaseDate.ToString("yyyy.MM.dd.") + upgrade.IssueNumber + " " + upgrade.IssueTitle;
                            if (name.Length > 50)
                                name = name.Remove(50);
                            return name;
                        });
                        versionCombobox.Values = upgradeNames.ToArray();
                    }
                    finally
                    {
                        presenter.MainPresenter.ShowWaitCursor(false);
                    }
                }
            }

            if (apsimTypeToRunCombobox.SelectedValue == "A directory")
                directoryLabel.Text = "Directory:";
            else
                directoryLabel.Text = "Zip file:";
        }
        
        /// <summary>Perform the actual submission of a job to the cloud.</summary>
        private async Task SubmitJob(object sender, EventArgs args)
        {
            JobParameters job = new JobParameters
            {
                ID = Guid.NewGuid(),
                DisplayName = nameOfJobEdit.Text,
                Model = modelToRun,
                ApsimXPath = directoryEdit.Text,
                CpuCount = Convert.ToInt32(numberCPUCombobox.SelectedValue),
                ModelPath = Path.GetTempPath() + Guid.NewGuid(),
                CoresPerProcess = 1,
                ApsimXVersion = Path.GetFileName(directoryEdit.Text),
                JobManagerShouldSubmitTasks = true,
                AutoScale = true,
                MaxTasksPerVM = 16,
            };

            if (string.IsNullOrWhiteSpace(job.DisplayName))
                throw new Exception("A description is required");

            if (string.IsNullOrWhiteSpace(job.ApsimXPath))
                throw new Exception("Invalid path to apsim");

            if (!Directory.Exists(job.ApsimXPath) && !File.Exists(job.ApsimXPath))
                throw new Exception($"File or Directory not found: '{job.ApsimXPath}'");

            if (job.CoresPerProcess <= 0)
                job.CoresPerProcess = 1;

            if (job.SaveModelFiles && string.IsNullOrWhiteSpace(job.ModelPath))
                throw new Exception($"Invalid model output directory: '{job.ModelPath}'");

            if (!Directory.Exists(job.ModelPath))
                Directory.CreateDirectory(job.ModelPath);

            try
            {
                await cloudInterface.SubmitJobAsync(job, cancellation.Token, s =>
                {
                    view.InvokeOnMainThread(delegate
                    {
                        statusLabel.Text = s;
                    });
                });
            }
            catch (Exception err)
            {
                statusLabel.Text = "Cancelled";
                presenter.MainPresenter.ShowError(err);
            }
        }
    }
}