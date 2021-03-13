using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion // Properties

        #region Constructor
        public HomePageViewModel()
        {

        }
        #endregion // Constructor
    }
}
