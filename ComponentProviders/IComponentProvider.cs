using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal interface IComponentProvider<T>
    {
        public Task<T> GetComponentAsync();
    }
}
   