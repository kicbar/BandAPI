using System;
using System.Collections.Generic;

namespace BandAPI.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperty { get; set; }
        public bool Revert { get; set; }
        
        public PropertyMappingValue(IEnumerable<string> destinationProperty, bool revert = false)
        {
            DestinationProperty = destinationProperty ??
                throw new ArgumentNullException(nameof(destinationProperty));
            Revert = revert;
        }
    }
}
