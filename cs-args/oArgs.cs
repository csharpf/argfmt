using System.Collections.Generic;
using ISList = System.Collections.Generic.IList<string>;
using IOMap = System.Collections.Generic.IDictionary<string, object>;
using ISMap = System.Collections.Generic.IDictionary<string, string>;
using IFnMap = System.Collections.Generic.IDictionary<string, cs_args.oArgs.Fn>;

namespace cs_args {

	/// <summary>
	/// Get command-line arguments as a dictionary.
	/// </summary>
	class oArgs {

		// types
		/// <summary>
		/// Argument handler function.
		/// </summary>
		public delegate bool Fn(IOMap m, string k, string v);


		// static data
		/// <summary>
		/// Default zero-valued option handler.
		/// </summary>
		public static Fn Flag = new Fn(_Flag);
		/// <summary>
		/// Default one-valued option handler.
		/// </summary>
		public static Fn Option = new Fn(_Option);
		/// <summary>
		/// Default multi-valued option handler.
		/// </summary>
		public static Fn MultiOption = new Fn(_MultiOption);


		// functions
		/// <summary>
		/// Get command-line arguments as dictionary.
		/// </summary>
		/// <param name="m">Output argument map.</param>
		/// <param name="args">Command-line arguments</param>
		/// <param name="key">Key mapping for arguments.</param>
		/// <returns>Argument map.</returns>
		public static IOMap Get(IOMap m, string[] args, ISMap key) {
			return Get(m, args, key, new Dictionary<string, Fn>());
		}
		/// <summary>
		/// Get command-line arguments as dictionary.
		/// </summary>
		/// <param name="m">Output argument map.</param>
		/// <param name="args">Command-line arguments</param>
		/// <param name="key">Key mapping for arguments.</param>
		/// <param name="fn">Argument handler functions for each key.</param>
		/// <returns>Argument map.</returns>
		public static IOMap Get(IOMap m, string[] args, ISMap key, IFnMap fn) {
			for(int i=0, j; i<args.Length; i++) {
				string k = key.ContainsKey(k = args[i]) ? key[k] : "";
				Fn f = fn.ContainsKey(k) ? fn[k] : (k==""? Option : Flag);
				if (f(m, k, (j = k == "" ? i : i + 1) < args.Length ? args[j] : "")) i++;
			}
			return m;
		}

		/// <summary>
		/// Default zero-valued option handler function. Saves value as empty string.
		/// </summary>
		/// <param name="m">Output argument map.</param>
		/// <param name="k">Argument key.</param>
		/// <param name="v">Argument value.</param>
		/// <returns>False, indicating argument is a flag.</returns>
		private static bool _Flag(IOMap m, string k, string v) {
			m.Add(k, "");
			return false;
		}
		/// <summary>
		/// Default one-valued option handler function. Saves value as a string.
		/// </summary>
		/// <param name="m">Output argument map.</param>
		/// <param name="k">Argument key.</param>
		/// <param name="v">Argument value.</param>
		/// <returns>True, indicating argument is an option.</returns>
		private static bool _Option(IOMap m, string k, string v) {
			m.Add(k, v);
			return true;
		}
		/// <summary>
		/// Default multi-valued option handler function. Saves values as a list.
		/// </summary>
		/// <param name="m">Output argument map.</param>
		/// <param name="k">Argument key.</param>
		/// <param name="v">Argument value.</param>
		/// <returns>True, indicating argument is an option.</returns>
		private static bool _MultiOption(IOMap m, string k, string v) {
			ISList l = (l = (ISList)m[k]) == null ? (ISList)(m[k]=new List<string>()) : l;
			l.Add(v);
			return true;
		}
	}
}
