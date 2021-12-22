using System;
using System.Collections.Generic;
using System.Text;

namespace QR2Web
{
    public interface OSInterface
    {
        bool OpenExternalUrl(string url);
        void EnablePermissionLocation();
    }
}
