using System.Windows.Input;
using GalaSoft.MvvmLight;
using Crawler.Model;
using GalaSoft.MvvmLight.Command;

namespace Crawler.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public string ButtonContent { get { return "clickMe!"; } }
        public string ButtonStartCrawlContent { get { return "Start Crawl"; } }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });

            MyCommand = new RelayCommand(ExecuteMyCommand, _canExecuteMyCommand);
            StartCrawlCommand = new RelayCommand(ExecuteStartCrawlCommand, _canExecuteStartCrawl);
        }


        public RelayCommand MyCommand
        {
            get;
            set;
        }

        public RelayCommand StartCrawlCommand
        {
            get;
            set;
        }

        private void ExecuteMyCommand()
        {
            WelcomeTitle = "haha";
        }
        private bool _canExecuteMyCommand()
        {
            return true;
        }

        private void ExecuteStartCrawlCommand()
        {

        }

        private bool _canExecuteStartCrawl()
        {
            return true;
        }




    }
}