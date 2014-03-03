﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Team537.Scouting.Viewer.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Team537.Scouting.Viewer
{
    using Windows.Media.Capture;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Media.Imaging;

    using Team537.Scouting.Model;
    using Team537.Scouting.Viewer.ViewModels;

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class TeamDetails : Page
    {
        private NavigationHelper navigationHelper;
        private TeamDetailsViewModel defaultViewModel = new TeamDetailsViewModel();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public TeamDetailsViewModel DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public TeamDetails()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            object navigationParameter;
            if (e.PageState != null && e.PageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = e.PageState["SelectedItem"];
            }

            var team = e.NavigationParameter as Team;
            if (team == null)
            {
                var dialog = new MessageDialog("Invalid Team Selected");
                await dialog.ShowAsync();
                return;
            }

            this.defaultViewModel.Team = team;

            if (this.defaultViewModel.Team.ImagePath != null)
            {
                var frcFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("FRC", CreationCollisionOption.OpenIfExists);
                var yearFolder = await frcFolder.CreateFolderAsync("2014", CreationCollisionOption.OpenIfExists);
                var imageFile = await yearFolder.GetFileAsync(this.defaultViewModel.Team.ImagePath);

                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(await imageFile.OpenReadAsync());

                this.defaultViewModel.TeamImage = bitmapImage;
            }
            else
            {
                this.TeamImage.Source = new BitmapImage();
            }

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            // TODO: Assign the selected item to this.flipView.SelectedItem
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void TakePicture_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CaptureTeamPicture), this.DefaultViewModel.Team);
        }
    }
}
