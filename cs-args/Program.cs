using System;
using System.Collections.Generic;
using ISList = System.Collections.Generic.IList<string>;
using IOMap = System.Collections.Generic.IDictionary<string, object>;
using ISMap = System.Collections.Generic.IDictionary<string, string>;
using IFnMap = System.Collections.Generic.IDictionary<string, cs_args.oArgs.Fn>;

namespace cs_args {
	class Program {
		static void Main(string[] args) {
			IOMap m = new Dictionary<string, object> {
				["b"] = "banana",
				["c"] = new List<string> {"cream"},
			};
			ISMap key = new Dictionary<string, string> {
				["-a"] = "a", ["-b"] = "b", ["-c"] = "c",
				["--apple"] = "a", ["--banana"] = "b", ["--cake"] = "c"
			};
			IFnMap fn = new Dictionary<string, oArgs.Fn> {
				["b"] = oArgs.Option, ["c"] = oArgs.MultiOption
			};
			m = oArgs.Get(m, args, key, fn);
			Console.WriteLine(string.Join(";", m));
			Console.ReadKey();
		}
	}
}
