using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ApiRoutesOptions
    {
        public string BackendRoute { get; set; } = "localization-provider/backend";
        public string FrontendRoute { get; set; } = "localization-provider/frontend";
    }
}
