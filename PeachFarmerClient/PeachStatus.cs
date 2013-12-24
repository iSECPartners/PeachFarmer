using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class PeachStatus
    {
        public UInt64? LastCompletedIteration { get; set; }

        public UInt64? FinalIteration { get; set; }

        public bool Finished { get; set; }

        public DateTime? LastUpdate { get; set; }

        public PeachStatus()
        {
            LastCompletedIteration = null;
            FinalIteration = null;
            Finished = false;
            LastUpdate = null;
        }

        public PeachStatus(UInt64? lastCompletedIteration, UInt64? finalIteration, bool finished, DateTime? lastUpdate)
        {
            LastCompletedIteration = lastCompletedIteration;
            FinalIteration = finalIteration;
            Finished = finished;
            LastUpdate = lastUpdate;
        }

        public override bool Equals(object obj)
        {
            return Equals((PeachStatus)obj);
        }

        public bool Equals(PeachStatus other)
        {
            if (this.LastCompletedIteration != other.LastCompletedIteration)
            {
                return false;
            }

            if (this.FinalIteration != other.FinalIteration)
            {
                return false;
            }

            if (this.Finished != other.Finished)
            {
                return false;
            }

            if (this.LastUpdate != other.LastUpdate)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
