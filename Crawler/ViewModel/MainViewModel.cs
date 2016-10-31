using System;
using System.Threading;
using System.Windows.Input;
using Abot.Core;
using Abot.Poco;
using GalaSoft.MvvmLight;
using Crawler.Model;
using Crawler.Model.Abot;
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
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;
        private string _crawlUrl = string.Empty;
        private string _localFolder = string.Empty;
        private int _maxDepth;
        private int _maxPages;

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

        public string CrawlUrl
        {
            get { return _crawlUrl; }
            set { Set(ref _crawlUrl, value); }
        }

        public string LocalFolder
        {
            get { return _localFolder; }
            set { Set(ref _localFolder, value); }
        }

        public int MaxDepth
        {
            get { return _maxDepth; }
            set { Set(ref _maxDepth, value); }
        }

        public int MaxPages
        {
            get { return _maxPages; }
            set { Set(ref _maxPages, value); }
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

            _syncContext = SynchronizationContext.Current;

            CrawlUrl = "http://www.poesifestivalen.no/";
            LocalFolder = "C:/Crawl/";
            MaxDepth = 10;
            MaxPages = 10;

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
            try
            {
                CrawlConfiguration crawlConfig = AbotConfigurationSectionHandler.LoadFromXml().Convert();
                crawlConfig.MaxCrawlDepth = Convert.ToInt32(MaxDepth);//this overrides the config value
                crawlConfig.MaxPagesToCrawl = Convert.ToInt32(MaxPages);//this overrides the config value

                WelcomeTitle = string.Empty;
                AbotManager m = new AbotManager();
                m.MessageUpdate += M_MessageUpdate;
                m.RunCrawl(_crawlUrl, _localFolder, crawlConfig);
            }
            catch (Exception ex)
            {
                M_MessageUpdate(this, new MessageEventArgs(ex.Message));
            }
        }

        private void M_MessageUpdate(object sender, MessageEventArgs args)
        {
            WelcomeTitle += "\n" + args.Message;
        }

        private bool _canExecuteStartCrawl()
        {
            return true;
        }
    }
}