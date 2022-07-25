using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DJT.AspNetCore.Mvc;

namespace DJT.AspNetCore
{
    /// <summary>
    /// Extension methods for HttpResponseMessage
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Convert HttpResponseMessage to general implementation of IActionResult
        /// for controller action responses.
        /// </summary>
        /// <param name="msg">The HttpResponseMessage to implement in the response</param>
        /// <returns>General implementation of IActionResult</returns>
        public static HttpResponseMessageResult ToActionResult(this HttpResponseMessage msg)
        {
            return new HttpResponseMessageResult(msg);
        }
    }
}
