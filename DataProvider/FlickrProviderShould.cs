using System;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Threading;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using flickr_app.Provider;
using flickr_app.Models;

namespace flick_app.TEST
{

    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("jsonFlickrFeed({\"title\":\"Uploadsfromeveryone\",\"link\":\"https://www.flickr.com/photos/\",\"items\":[{\"title\":\"DSC01917\",\"link\":\"link\",\"media\":{\"m\":\"media\"},\"tags\":\"\"}]})")
            };

            return await Task.FromResult(responseMessage);
        }
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string str)
        {
            return new HttpClient(new HttpMessageHandlerStub());
        }
    }

    [TestClass]
    public class FlickrProviderShould
    {
        private FlickProvider prov;
        public FlickrProviderShould()
        {
            prov = new FlickProvider(new HttpClientFactory());
        }

        [TestMethod]
        public async Task ParseCorrectlyTheResponse()
        {
            IEnumerable<FlickrFeedItem> results = await prov.GetImagesFromFlickr(null);
            Debug.WriteLine(results);
            Assert.AreEqual(results.Count(), 1, "No results returned");
            
            FlickrFeedItem item = results.First();
            Assert.AreEqual(item.title, "DSC01917", "FlickrFeedItem item not parsed correctly");
            Assert.AreEqual(item.link, "link", "FlickrFeedItem link not parsed correctly");
        }
    }
}
