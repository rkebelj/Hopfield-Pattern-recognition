using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopffield.Network
{
    public class Neuron
    {
        private int _state;
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }


        public Neuron()
        {
            int r = new Random().Next(2);
            switch (r)
            {
                case 0: _state = NeuronStates.AlongField; break;
                case 1: _state = NeuronStates.AgainstField; break;
            }
        }


        public bool ChangeState(Double field)
        {
            bool res = false;
            if (field * this.State < 0)
            {
                this._state = -this._state;
                res = true;
            }
            return res;
        }
    }
    public static class NeuronStates
    {
        public static int AlongField = 1;
        public static int AgainstField = -1;
    }
}
