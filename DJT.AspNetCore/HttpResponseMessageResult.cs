using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace DJT.AspNetCore.Mvc
{
    /// <summary>
    /// Provides for HttpResponseMessage as response to controller actions.
    /// </summary>
    public class HttpResponseMessageResult : IActionResult
    {
        private readonly HttpResponseMessage _responseMessage;

        /// <summary>
        /// Constructor, requires an HttpResponseMessage
        /// </summary>
        /// <param name="responseMessage">The message to provide</param>
        public HttpResponseMessageResult([NotNull]HttpResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }

        /// <summary>
        /// Provides HTTP response for HttpResponseMessage as part of action result
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task ExecuteResultAsync([NotNull]ActionContext context)
        {
            var response = context.HttpContext.Response;
            if (_responseMessage == null)
                throw new InvalidOperationException("Null HttpResponseMessage unacceptable!");

            using (_responseMessage) 
            {
                response.StatusCode = (int)_responseMessage.StatusCode;
                var responseFeature = context.HttpContext.Features.Get<IHttpResponseFeature>();
                if (responseFeature != null)
                {
                    responseFeature.ReasonPhrase = _responseMessage.ReasonPhrase;
                }

                var headers = _responseMessage.Headers;

                //Ignore the Transfer-Encoding header if it is "chunked".
                //Let the host decide if the response should be chunked or not.
                if (headers.TransferEncodingChunked == true && headers.TransferEncoding.Count == 1)
                {
                    headers.TransferEncoding.Clear();
                }

                foreach(var h in headers)
                {
                    response.Headers.Add(h.Key, h.Value.ToArray());
                }

                if (_responseMessage.Content != null)
                {
                    var contentHeaders = _responseMessage.Content.Headers;
                    //Apparently, headers are lazy loaded after computing the
                    //content length.  Thus we'll make use of it here.
                    if (contentHeaders.ContentLength > 0)
                    {
                        foreach(var h in contentHeaders)
                        {
                            response.Headers.Add(h.Key, h.Value.ToArray());
                        }
                    }
                    await _responseMessage.Content.CopyToAsync(response.Body);
                }

            }
        }
    }
}