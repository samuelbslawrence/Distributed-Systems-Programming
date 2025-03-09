using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    interface IFilter
    {
        public IMessage Run(IMessage message);
    }
}
