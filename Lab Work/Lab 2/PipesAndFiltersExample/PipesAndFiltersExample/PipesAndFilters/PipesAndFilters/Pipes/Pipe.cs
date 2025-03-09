using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    class Pipe : IPipe
    {
        private List<IFilter> Filters { get; set; }
        public Pipe()
        {
            Filters = new List<IFilter>();
        }

        public IMessage ProcessMessage(IMessage message)
        {
            foreach(IFilter filter in Filters)
            {
                message = filter.Run(message);
            }
            return message;
        }

        public void RegisterFilter(IFilter filter)
        {
            Filters.Add(filter);
        }
    }
}
