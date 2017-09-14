using System.Collections.Generic;
using WebOptimizer;
using WebOptimizer.AutoPrefixer;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods for registrating the auto prefixer on the Asset Pipeline.
    /// </summary>
    public static class AutoPrefixerPipelineExtensions
    {
        /// <summary>
        /// Add missing vendor prefixes to CSS files
        /// </summary>
        public static IAsset AutoPrefixCss(this IAsset asset)
        {
            return asset.AutoPrefixCss("> 1%");
        }

        /// <summary>
        /// Add missing vendor prefixes to CSS files
        /// </summary>
        public static IAsset AutoPrefixCss(this IAsset asset, params string[] browsers)
        {
            asset.Processors.Add(new AutoPrefixerProcessor(browsers));
            return asset;
        }

        /// <summary>
        /// Add missing vendor prefixes to CSS files
        /// </summary>
        public static IEnumerable<IAsset> AutoPrefixCss(this IEnumerable<IAsset> assets)
        {
            return assets.AutoPrefixCss(new string[0]);
        }

        /// <summary>
        /// Add missing vendor prefixes to CSS files
        /// </summary>
        public static IEnumerable<IAsset> AutoPrefixCss(this IEnumerable<IAsset> assets, params string[] browsers)
        {
            var list = new List<IAsset>();

            foreach (IAsset asset in assets)
            {
                list.Add(asset.AutoPrefixCss(browsers));
            }

            return list;
        }
    }
}
