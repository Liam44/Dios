using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dios.Exceptions
{
    public class NavigationPropertiesWrapperUndefinedException:Exception
    {
        public NavigationPropertiesWrapperUndefinedException(): base("NavigationPropertiesWrapper is null in NavigationProperties.")
        {

        }
    }
}
