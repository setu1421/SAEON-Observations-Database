using Force.Crc32;
using System;
using System.Text;

namespace SAEON.Observations.Core
{
    public class StableHash
    {
        private int hashCode;
        public int HashCode { get { return hashCode; } }

        public void Add(string value)
        {
            unchecked
            {
                hashCode += CalculateCrc32(Encoding.UTF8.GetBytes(value));
            }
        }

        public void Add(double? value)
        {
            if (value.HasValue)
                unchecked
                {
                    hashCode += CalculateCrc32(BitConverter.GetBytes(value.Value));
                }
        }

        public void Add(Guid value)
        {
            unchecked
            {
                hashCode += CalculateCrc32(value.ToByteArray());
            }
        }

        public void Add(DateTime? value)
        {
            if (value.HasValue)
                unchecked
                {
                    hashCode += CalculateCrc32(BitConverter.GetBytes(value.Value.Ticks));
                }
        }

        private int CalculateCrc32(byte[] data)
        {
            return (int)Crc32Algorithm.Compute(data);
        }
    }
}
