using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using WebOptimizer.AutoPrefixer;
using Xunit;

namespace WebOptimizer.AutoPrefixer.Test
{
    public class AssetExtensionsTest
    {
        [Fact]
        public void Compile_single_Success()
        {
            var asset = GenerateAssets(1).First();

            asset.AutoPrefixCss();

            Assert.Equal(1, asset.Processors.Count);
            Assert.True(asset.Processors.Any(p => p is AutoPrefixerProcessor));
        }

        [Fact]
        public void Compile_Multiple_Success()
        {
            var assets = GenerateAssets(5).ToArray();

            assets.AutoPrefixCss();

            foreach (IAsset asset in assets)
            {
                Assert.Equal(1, asset.Processors.Count);
                Assert.True(asset.Processors.Any(p => p is AutoPrefixerProcessor));
            }
        }

        private IEnumerable<IAsset> GenerateAssets(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                var asset = new Mock<IAsset>();
                asset.SetupGet(a => a.Processors)
                     .Returns(new List<IProcessor>());

                yield return asset.Object;
            }
        }
    }
}
