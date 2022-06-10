using Hopffield.Network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Colors = System.Windows.Media.Colors;
using Color = System.Drawing.Color;
using System.Windows.Threading;
using System.IO;
using System.Drawing.Imaging;

namespace Hopffield
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		bool _mouseDown = false;
		bool _drawWhite = false;
		DrawingBoard Board;
		private NeuralNetwork NN;
		private int imageDim = 10;

		IEnumerable<System.Windows.Shapes.Rectangle> rectangle;

		public MainWindow()
		{
			InitializeComponent();
			Board = new DrawingBoard(imageDim);
			DataContext = Board;
			NN = new NeuralNetwork(imageDim * imageDim);
			NN.EnergyChanged += new EnergyChangedHandler(UpdateBoard);
			rectangle = FindVisualChilds<System.Windows.Shapes.Rectangle>(board);
			

		}

		private void Draw(object sender, MouseEventArgs e)
		{
			var cell = ((System.Windows.Shapes.Rectangle)sender).DataContext as Cell;
			if (_mouseDown)
			{
				cell.IsInked = true;
				if (_drawWhite)
					((System.Windows.Shapes.Rectangle)sender).Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
				else
					((System.Windows.Shapes.Rectangle)sender).Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

				//((System.Windows.Shapes.Rectangle)sender).Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
			}
			Debug.WriteLine($"row: {cell.Row} | column: {cell.Column} | mouse DOWN: {_mouseDown}");

		}

		private void MouseDown(object sender, MouseButtonEventArgs e)
		{

			_mouseDown = true;
		}

		private void MouseUp(object sender, MouseButtonEventArgs e)
		{
			_mouseDown = false;
		}
		private void DrawSingle(object sender, MouseButtonEventArgs e)
		{
			var cell = ((System.Windows.Shapes.Rectangle)sender).DataContext as Cell;
			if (_drawWhite)
			{
				((System.Windows.Shapes.Rectangle)sender).Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
				cell.IsInked = false;

			}
			else
			{
				((System.Windows.Shapes.Rectangle)sender).Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
				cell.IsInked = false;

			}
		}

		private IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj == null) yield return (T)Enumerable.Empty<T>();
			for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				DependencyObject ithChild = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
				if (ithChild == null) continue;
				if (ithChild is T t) yield return t;
				foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
			}

		}
		private void Clear(object sender, MouseButtonEventArgs e)
		{
			((DrawingBoard)DataContext).Cells.ForEach(cell =>
			{
				cell.IsInked = false;

			});
			foreach (System.Windows.Shapes.Rectangle tb in rectangle)
			{
				tb.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
			}

		}


		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var cells = Board.Cells;
		}


		private void AddPatternFromFile(object sender, RoutedEventArgs e)
		{
			Image imgPattern;

			OpenFileDialog op = new OpenFileDialog();
			op.Title = "Select a picture";
			if (op.ShowDialog() == true)
			{
				imgPattern = Image.FromFile(op.FileName);
				if (imgPattern.Width != imageDim || imgPattern.Height != imageDim)
				{
					MessageBox.Show("wrong image size");
					return;
				}
				int[,] patternPixels;
				int p = 0;
				int midColor = Math.Abs((int)(Color.Black.ToArgb() / 2));
				Bitmap b = new Bitmap(imgPattern);
				patternPixels = new int[imageDim, imageDim];
				List<Neuron> pattern = new List<Neuron>(imageDim * imageDim);
				for (int i = 0; i < imageDim; i++)
					for (int j = 0; j < imageDim; j++)
					{
						Neuron n = new Neuron();
						p = Math.Abs(b.GetPixel(i, j).ToArgb());

						int index = j * imageDim + i;
						//var element = rectangle.ElementAt(index);


						if (p > midColor)
						{
							//b.SetPixel(i, j, Color.Black);
							//n.State = NeuronStates.AgainstField;
							Board.Cells[index].IsInked = true;
							rectangle.ElementAt(index).Fill = new SolidColorBrush(Colors.Black);
						}
						else
						{
							//b.SetPixel(i, j, Color.White);
							//n.State = NeuronStates.AlongField;
							Board.Cells[index].IsInked = false;
							rectangle.ElementAt(index).Fill = new SolidColorBrush(Colors.White);
						}
						//pattern.Add(n);
					}
				//NN.AddPattern(pattern);



			}
		}
		private void SaveToImage()
		{

			var bitmap = new Bitmap(imageDim, imageDim);

			for (int i = 0; i < imageDim; i++)
				for (int j = 0; j < imageDim; j++)
				{
					int index = i * imageDim + j;
					bool white = !Board.Cells.ElementAt(index).IsInked;

					if (white)
						bitmap.SetPixel(i, j, Color.White);
					else
						bitmap.SetPixel(i, j, Color.Black);

				}

			bitmap.RotateFlip(RotateFlipType.Rotate270FlipY);
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = Directory.GetCurrentDirectory();
			//dialog.DefaultExt = "bmp";
			dialog.FileName = $"untitled{imageDim}";
			dialog.Filter = "BitmapImage (*.bmp)|*.bmp";
			if (dialog.ShowDialog() == true)
			{
				bitmap.Save(dialog.SafeFileName);

			}





		}

		private void AddPatternFromBoard(object sender, EventArgs e)
		{

			List<Neuron> pattern = new List<Neuron>(imageDim * imageDim);
			for (int i = 0; i < imageDim; i++)
				for (int j = 0; j < imageDim; j++)
				{
					Neuron n = new Neuron();
					int index = i * imageDim + j;
					if (!Board.Cells.ElementAt(index).IsInked)
					{
						//white
						n.State = NeuronStates.AlongField;
					}
					else
					{
						//black
						n.State = NeuronStates.AgainstField;
					}
					pattern.Add(n);
				}
			NN.AddPattern(pattern);
			//SaveToImage();
			paterns.Content = NN.PatternsStored.ToString();
		}


		private void Run(object sender, RoutedEventArgs e)
		{
			List<Neuron> initialState = new List<Neuron>(imageDim * imageDim);
			for (int i = 0; i < imageDim; i++)
				for (int j = 0; j < imageDim; j++)
				{
					Neuron neuron = new Neuron();
					int index = i * imageDim + j;
					if (!Board.Cells.ElementAt(index).IsInked)
						neuron.State = NeuronStates.AlongField;
					else
						neuron.State = NeuronStates.AgainstField;
					initialState.Add(neuron);
				}

			NN.Run(initialState);
			energy.Content = NN.Energy.ToString();

		}

		private void ChangeSize(object sender, RoutedEventArgs e)
		{
			imageDim = Int32.Parse(size.Text);
			Board = new DrawingBoard(imageDim);
			NN = new NeuralNetwork(imageDim * imageDim);
			NN.EnergyChanged += new EnergyChangedHandler(UpdateBoard);
			DataContext = Board;
			rectangle = FindVisualChilds<System.Windows.Shapes.Rectangle>(board);
		}

		private void UpdateBoard(object sender, EnergyEventArgs args)
		{
			int index = args.NeuronIndex;
			energy.Content = args.Energy.ToString();
			var element = rectangle.ElementAt(index);

			var la = ((SolidColorBrush)element.Fill).Color == Colors.Black ? Colors.White : Colors.Black;

			rectangle.ElementAt(index).Fill = new SolidColorBrush(la);

			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
			{
				frame.Continue = false;
				return null;
			}), null);

			Dispatcher.PushFrame(frame);
			//EDIT:
			Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
										  new Action(delegate { }));
		}



		private void ChangeInkColour(object sender, RoutedEventArgs e)
		{
			_drawWhite = !_drawWhite;

		}

		private void SavePatternToFile(object sender, RoutedEventArgs e)
		{
			SaveToImage();

		}
	}
}
