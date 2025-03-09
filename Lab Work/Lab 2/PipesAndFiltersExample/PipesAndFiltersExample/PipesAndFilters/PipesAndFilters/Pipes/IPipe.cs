using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    interface IPipe
    {
        public void RegisterFilter(IFilter filter);
        public IMessage ProcessMessage(IMessage message);
    }
}
