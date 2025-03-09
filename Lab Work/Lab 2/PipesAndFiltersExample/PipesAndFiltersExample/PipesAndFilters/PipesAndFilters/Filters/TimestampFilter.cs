using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    class TimestampFilter : IFilter
    {
        public IMessage Run(IMessage message)
        {
            message.Headers.Add("Timestamp", DateTime.Now.ToString());
            return message;
        }
    }
}
