using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AirQuality.Domain.Feed;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AirQuality
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var parser = new FeedParser();
            var item = parser.Parse(116, FeedType.Rss);
            OnApplyTemplate();
            Location.Text = item.Location;
            Agency.Text = item.Agency;
            LastUpdate.Text = item.LastUpdate;
            ParticlePollution.Text = item.ParticlePollution;
            Ozone.Text = item.Ozone;
        }
    }
}