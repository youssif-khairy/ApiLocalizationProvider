using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.DTO
{
    public class MemoryCacheKey : IEquatable<MemoryCacheKey>
    {
        public string ResourceName { get; set; }
        public string Culture { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (MemoryCacheKey)obj;
            return ResourceName == other.ResourceName && Culture == other.Culture;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + (ResourceName != null ? ResourceName.GetHashCode() : 0);
                hash = hash * 23 + (Culture != null ? Culture.GetHashCode() : 0);
                return hash;
            }
        }

        public bool Equals(MemoryCacheKey other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return GetHashCode() == other.GetHashCode();
        }

        public static bool operator ==(MemoryCacheKey lhs, MemoryCacheKey rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(MemoryCacheKey lhs, MemoryCacheKey rhs)
        {
            return !(lhs == rhs);
        }
    }

}
