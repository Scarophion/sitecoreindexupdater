using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreTools.IndexUpdate
{
    public class OpenCommand: Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            UrlString urlString = new UrlString()
            {
                Path = "/sitecore/client/your-apps/indexupdate/"
            };
            SheerResponse.Eval($"window.open('{urlString}')");
        }
    }
}
