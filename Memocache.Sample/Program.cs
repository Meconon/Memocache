using Meconon.Memocache;
using System;
using System.Diagnostics;

namespace Memocached.Sample
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			// Decide before anything else whether we want debugging on or not.

			Stopwatch timer = new Stopwatch();

			Console.WriteLine("Creating sample class");
			timer.Reset();
			timer.Start();
			using (SampleClass c = new SampleClass())
			{
				timer.Stop();
				Console.WriteLine("Creation of class took {0} milleseconds\n\n", timer.ElapsedMilliseconds);

				Console.WriteLine("Calling with parameters (2, 2) ...");
				timer.Reset();
				timer.Start();
				long result = c.ComplexCalculation(2, 2);
				timer.Stop();

				Console.WriteLine("Giving a result of : {0} and took {1} milliseconds\n\n", result, timer.ElapsedMilliseconds);

				Console.WriteLine("Calling with parameters (3, 2) ...");
				timer.Reset();
				timer.Start();
				result = c.ComplexCalculation(3, 2);
				timer.Stop();
				Console.WriteLine("Giving a result of : {0} and took {1} milliseconds\n\n", result, timer.ElapsedMilliseconds);

				Console.WriteLine("Calling a second time with parameters (2, 2).  This will use cache ...");
				timer.Reset();
				timer.Start();
				result = c.ComplexCalculation(2, 2);
				timer.Stop();
				Console.WriteLine("Giving a result of : {0} and took {1} milliseconds\n\n", result, timer.ElapsedMilliseconds);
			}

			Console.WriteLine("Press Enter to close");
			Console.ReadLine();
		}
	}
}