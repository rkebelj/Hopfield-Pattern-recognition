using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopffield.Network
{
    public class EnergyEventArgs : EventArgs
    {
        private double energy;
        private int neuronIndex;
        public double Energy
        {
            get { return energy; }
        }
        public EnergyEventArgs(double Energy, int NeuronIndex)
        {
            this.energy = Energy;
            this.neuronIndex = NeuronIndex;

        }
        public int NeuronIndex
        {
            get { return neuronIndex; }
        }
    }
}
