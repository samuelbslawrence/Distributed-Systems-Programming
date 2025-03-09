using System;
using System.Collections.Generic;
using System.Text;

namespace PipesAndFilters
{
    class AuthenticateFilter : IFilter
    {
        public IMessage Run(IMessage message)
        {
            if(message.Headers.ContainsKey("User") && int.TryParse(message.Headers["User"], out int id))
            {
                ServerEnvironment.SetCurrentUser(id);
            }
            return message;
        }
    }
}
