using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	class Program
	{
		private static readonly string Foo = "Cows";
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World");
			Console.WriteLine(Foo);
			Baz();
			Console.ReadKey();
		}

		static void Baz(string hi = "Test")
		{
			Console.WriteLine(hi);
		}
	}
}
