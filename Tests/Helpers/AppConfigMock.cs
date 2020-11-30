using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Binders;

namespace Tests.Helpers
{
    public class AppConfigMock
    {
        public static  BaseUrls GetBaseUrls()
        {
            return new BaseUrls() {
                BinListAPI = "https://lookup.binlist.net"
            };
        }
    }
}
