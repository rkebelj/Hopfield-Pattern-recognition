using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Hopffield.Network
{
	public delegate void EnergyChangedHandler(object sender, EnergyEventArgs e);


	public class NeuralNetwork
	{
		private List<Neuron> neurons;

		private int _numOfNeurons;
		private int _patternsStored;
		private double _energy;
		private int[,] _matrix;

		private void CalculateEnergy()
		{
			double tempE = 0;
			for (int i = 0; i < _numOfNeurons; i++)
				for (int j = 0; j < _numOfNeurons; j++)
					if (i != j)
						tempE += _matrix[i, j] * neurons[i].State * neurons[j].State;
			_energy = -1 * tempE / 2;
		}
		public int NumOfNeurons
		{
			get { return _numOfNeurons; }
		}
		public int PatternsStored
		{
			get { return _patternsStored; }
		}
		public double Energy
		{
			get { return _energy; }
		}
		public int[,] Matrix
		{
			get { return _matrix; }
		}
		public List<Neuron> Neurons
		{
			get { return neurons; }
		}
		public NeuralNetwork(int n)
		{
			this._numOfNeurons = n;
			neurons = new List<Neuron>(n);
			for (int i = 0; i < n; i++)
			{
				Neuron neuron = new Neuron();
				neuron.State = 0;
				neurons.Add(neuron);
			}

			_matrix = new int[n, n];
			_patternsStored = 0;

			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
				{
					_matrix[i, j] = 0;
				}
		}
		public void AddPattern(List<Neuron> Pattern)
		{
			//vrednosti med seboj seštevamo
			for (int i = 0; i < _numOfNeurons; i++)
				for (int j = 0; j < _numOfNeurons; j++)
				{
					if (i == j) _matrix[i, j] = 0;
					else _matrix[i, j] += (Pattern[i].State * Pattern[j].State);
				}
			_patternsStored++;
		}

		public void Run(List<Neuron> initialState)
		{
			_energy = 0;
			this.neurons = initialState;
			int k = 1;
			int h = 0;
			while (k != 0)
			{
				k = 0;
				for (int i = 0; i < _numOfNeurons; i++)
				{
					h = 0;
					for (int j = 0; j < _numOfNeurons; j++)
						h += _matrix[i, j] * (neurons[j].State);

					if (neurons[i].ChangeState(h))
					{
						k++;
						CalculateEnergy();						
						OnEnergyChanged(new EnergyEventArgs(_energy, i));
					}
				}
			}
			CalculateEnergy();

		}

		public event EnergyChangedHandler EnergyChanged;

		protected virtual void OnEnergyChanged(EnergyEventArgs e)
		{
			if (EnergyChanged != null)
				EnergyChanged(this, e);																								
		}																																							
	}
}
