﻿using System;

namespace cs_args {
	class Program {
		static void Main(string[] args) {
			for (int i = 0; i < args.Length; i++)
				Console.WriteLine("\""+args[i]+"\"");
			Console.ReadKey();
		}
	}
}
