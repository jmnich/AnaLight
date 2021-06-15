using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Factories;
using AnaLight.Containers;
using System.Collections.ObjectModel;

namespace AnaLight.ViewModels
{
    public class HomePageViewModel : TabViewModel
    {
        private readonly static string anaLightRepoAddress = "https://github.com/jmnich/AnaLight";
        private readonly static string welcomeText = "Welcome to AnaLight";

        #region Properties
        /// <summary>
        /// Analight GitHub repository address.
        /// </summary>
        public Uri AnaLightGitHubRepo
        {
            get
            {
                return new Uri(HomePageViewModel.anaLightRepoAddress);
            }
        }

        public string WelcomeText
        {
            get
            {
                return welcomeText;
            }
        }

        /// <summary>
        /// A list of available device control panel types.
        /// </summary>
        public ObservableCollection<PanelInfo> AvailableControlPanels { get; }

        /// <summary>
        /// A list of available utility panel types.
        /// </summary>
        public ObservableCollection<PanelInfo> AvailableUtilityPanels { get; }

        #endregion // Properties

        #region MVVM Model
        /// <summary>
        /// MVVM Model for this VM.
        /// </summary>
        private Models.HomePageModel Model { get; }
        #endregion // MVVM Model


        #region Constructor
        public HomePageViewModel()
        {
            Model = new Models.HomePageModel();

            AvailableControlPanels = PanelFactory.AvailableControlPanels;
            AvailableUtilityPanels = PanelFactory.AvailableUtilityPanels;
        }
        #endregion // Constructor
    }
}
