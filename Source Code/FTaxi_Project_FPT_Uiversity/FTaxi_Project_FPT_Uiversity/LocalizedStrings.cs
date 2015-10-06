using FTaxi_Project_FPT_Uiversity.Resources;

namespace FTaxi_Project_FPT_Uiversity
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}