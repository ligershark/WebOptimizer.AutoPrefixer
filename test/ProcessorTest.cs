using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace WebOptimizer.AutoPrefixer.Test
{
    public class ProcessorTest
    {
        private static bool isDeleted;
        static ProcessorTest()
        {
            if (!isDeleted)
            {
                string dir = new AutoPrefixerProcessor().InstallDirectory;
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                    isDeleted = true;
                }
            }
        }

        [Theory]
        [InlineData("*{-webkit-transform:rotate(1deg);transform:rotate(1deg)}", new string[0])]
        [InlineData("*{-webkit-transform:rotate(1deg);-ms-transform:rotate(1deg);transform:rotate(1deg)}", new[] { "ie 9", "> 1%" })]
        [InlineData("*{transform:rotate(1deg)}", new[] { "last 2 edge versions" })]
        public async Task Compile_Success(string output, string[] browsers)
        {
            var processor = new AutoPrefixerProcessor(browsers);
            var pipeline = new Mock<IAssetPipeline>().SetupAllProperties();
            var context = new Mock<IAssetContext>().SetupAllProperties();
            var asset = new Mock<IAsset>().SetupAllProperties();
            var env = new Mock<IHostingEnvironment>();

            context.Object.Content = new Dictionary<string, byte[]> {
                { "/file.css", "*{transform:rotate(1deg)}".AsByteArray() },
            };

            context.Setup(s => s.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)))
                  .Returns(env.Object);

            var header = new StringValues("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/601.2.7 (KHTML, like Gecko) Version/9.0.1 Safari/601.2.7");
            context.Setup(c => c.HttpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out header))
                   .Returns(true);

            context.SetupGet(s => s.Asset)
                        .Returns(asset.Object);

            await processor.ExecuteAsync(context.Object);
            var result = context.Object.Content.First().Value;

            Assert.Equal(output, result.AsString().Trim());
        }

        [Theory]
        [InlineData("Safari", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/601.2.7 (KHTML, like Gecko) Version/9.0.1 Safari/601.2.7")]
        [InlineData("Explorer", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; Touch; rv:11.0) like Gecko")]
        [InlineData("Firefox", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0")]
        [InlineData("Opera", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36 OPR/47.0.2631.80")]
        [InlineData("Edge", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063")]
        [InlineData("Chrome", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36")]
        public void GetBrowserName(string name, string ua)
        {
            var request = new Mock<HttpRequest>();

            var header = new StringValues(ua);
            request.Setup(r => r.Headers.TryGetValue(HeaderNames.UserAgent, out header))
                   .Returns(true);

            var result = AutoPrefixerProcessor.GetBrowserName(request.Object);
            Assert.Contains(name, result);

        }
    }
}
