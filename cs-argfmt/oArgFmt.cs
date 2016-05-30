using System;
using System.Collections.Generic;

namespace orez.argfmt {
	class oArgFmt {

		// types
		public delegate int Fn(oArgFmt o, IList<object> dst, string di, IList<string> src, int si);

		// data
		/// <summary>
		/// List of parameters, with associated functions.
		/// </summary>
		public IList<IList<object>> Fmt;
		/// <summary>
		/// Long prefix of parameter.
		/// </summary>
		public string LongPrefix = "--";
		/// <summary>
		/// Short prefix of parameter.
		/// </summary>
		public string ShortPrefix = "-";

		/// <summary>
		/// Create an argument format object.
		/// </summary>
		/// <param name="fmt">List of parameters, with associated functions.</param>
		/// <param name="longPrefix">Optional. Long prefix of parameter (default = "--").</param>
		/// <param name="shortPrefix">Optional. Short prefix of parameter (default = "-").</param>
		public oArgFmt(IList<IList<object>> fmt, string longPrefix = "--", string shortPrefix = "-") {
			Fmt = fmt;
			LongPrefix = longPrefix;
			ShortPrefix = shortPrefix;
		}

		/// <summary>
		/// Saves the arguments into an indexed list.
		/// </summary>
		/// <param name="d">Output indexed list</param>
		/// <param name="args">Arguments array</param>
		public void Get(IList<object> d, IList<string> args) {
			for(int i = 0; i < args.Count; i++)
				i += Arg(this, d, args[i], args, i+1);
		}


		// delegates
		/// <summary>
		/// Saves parameter based on its type (<code>LongPrefix</code>, <code>ShortPrefix</code>).
		/// </summary>
		public Fn Arg = (o, d, di, s, si) => {
			int t = 0;
			if(di.StartsWith(o.LongPrefix)) return o.Param(o, d, di.Substring(o.LongPrefix.Length), s, si);
			else if(!di.StartsWith(o.ShortPrefix)) return o.Param(o, d, di, s, si);
			for(int i = o.ShortPrefix.Length; i < di.Length; i++)
				t += o.Param(o, d, di[i].ToString(), s, si);
			return t;
		};
		/// <summary>
		/// Saves parameter based on format-table <code>Fmt</code>.
		/// </summary>
		public Fn Param = (o, d, di, s, si) => {
			bool v = di == 0; string k = v ? "" : s[si];
			if(o.Fmt != null) for(int i = 0; i < o.Fmt.Count; i++) {
				IList<object> l = o.Fmt[i];
				if(l.Contains(k)) return ((Fn)l[l.Count - 1])(o, d, v ? 0 : i, s, v ? si : si + 1);
			}
			o.List(o, d, d.Count - 1, s, si);
			return 0;
		};

		/// <summary>
		/// Saves flag parameter (no value) as empty string at flag character.
		/// </summary>
		public Fn Flag = (o, d, di, s, si) => {
			d[di] = "";
			return 0;
		};
		/// <summary>
		/// Saves attribute parameter (1 value, overwrite) as valued string at attribute character.
		/// </summary>
		public Fn Attr = (o, d, di, s, si) => {
			d[di] = s[si];
			return 1;
		};
		/// <summary>
		/// Saves list parameter (1 value, append) as string list at list character.
		/// </summary>
		public Fn List = (o, d, di, s, si) => {
			IList<string> l = (l = (IList<string>)d[di]) == null ? (IList<string>)(d[di] = new List<string>()) : l;
			l.Add(s[si]);
			return 1;
		};
	}
}
