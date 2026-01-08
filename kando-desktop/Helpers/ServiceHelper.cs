using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Helpers
{
    public static class ServiceHelper
    {
        public static TService GetService<TService>()
            => IPlatformApplication.Current.Services.GetService<TService>();

        public static IServiceProvider Current
            => IPlatformApplication.Current.Services;
    }
}
