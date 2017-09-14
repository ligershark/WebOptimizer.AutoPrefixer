using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebOptimizer.NodeServices;

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
        /// Executes the asynchronous.
        /// </summary>
        public override async Task ExecuteAsync(IAssetContext context)
        {
            if (!EnsureNodeFiles("WebOptimizer.AutoPrefixer.node_files.zip"))
                return;

            var content = new Dictionary<string, byte[]>();
            string module = Path.Combine(InstallDirectory, "index.js");

            foreach (string route in context.Content.Keys)
            {
                var input = context.Content[route].AsString();
                var result = await NodeServices.InvokeAsync<string>(module, input, Browsers);

                content[route] = result.AsByteArray();
            }

            context.Content = content;
        }
    }
}
