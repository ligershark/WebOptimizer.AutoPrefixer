using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebOptimizer.NodeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace WebOptimizer.AutoPrefixer
{
    /// <summary>
    /// A LESS compiler
    /// </summary>
    /// <seealso cref="WebOptimizer.NodeServices.NodeProcessor" />
    public class AutoPrefixerProcessor : NodeProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoPrefixerProcessor"/> class.
        /// </summary>
        /// <param name="browsers">The browsers.</param>
        public AutoPrefixerProcessor(params string[] browsers)
        {
            Browsers = browsers;
        }

        /// <summary>
        /// An array of "browserlist" definitions.
        /// </summary>
        public IEnumerable<string> Browsers { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name => "AutoPrefixer";

        /// <summary>
        /// Gets the custom key that should be used when calculating the memory cache key.
        /// </summary>
        public override string CacheKey(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var ua))
            {
                return ua.ToString();
            }

            return base.CacheKey(context);
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        public override async Task ExecuteAsync(IAssetContext context)
        {
            if (!EnsureNodeFiles("WebOptimizer.AutoPrefixer.node_files.zip"))
                return;

            var content = new Dictionary<string, byte[]>();

            string module = Path.Combine(InstallDirectory, "index.js");
            var browserList = Browsers.Any() ? Browsers : new[] { GetBrowserName(context.HttpContext.Request) };

            foreach (string route in context.Content.Keys)
            {
                var input = context.Content[route].AsString();
                var result = await NodeServices.InvokeAsync<string>(module, input, browserList);

                content[route] = result.AsByteArray();
            }

            context.Content = content;
        }

        /// <summary>
        /// Gets the browser name based on the request user agent
        /// </summary>
        public static string GetBrowserName(HttpRequest request)
        {
            if (!request.Headers.TryGetValue(HeaderNames.UserAgent, out var ua))
                return "> 1%";

            string userAgent = ua.ToString();

            if (userAgent.Contains("Edge/"))
                return "Edge >= 12";

            if (userAgent.Contains("OPR/"))
                return "Opera >= 30";

            if (userAgent.Contains("Firefox/"))
                return "Firefox >= 26";

            if (userAgent.Contains("Trident/"))
                return "Explorer >= 8";

            if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome"))
                return "Safari >= 6";

            if (userAgent.Contains("Chrome"))
                return "Chrome >= 40";

            return "> 1%";
        }
    }
}
