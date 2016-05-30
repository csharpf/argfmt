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
			bool v = di.Length == 0; string k = v ? "" : s[si];
			if(o.Fmt != null) for(int i = 0; i < o.Fmt.Count; i++) {
				IList<object> l = o.Fmt[i];
				if(l.Contains(k)) return ((Fn)l[l.Count - 1])(o, d, v ? "" : ""+i, s, v ? si : si + 1);
			}
			o.List(o, d, ""+ (d.Count - 1), s, si);
			return 0;
		};

		/// <summary>
		/// Saves flag parameter (no value) as empty string at flag character.
		/// </summary>
		public Fn Flag = (o, d, di, s, si) => {
			d[di[0]] = "";
			return 0;
		};
		/// <summary>
		/// Saves attribute parameter (1 value, overwrite) as valued string at attribute character.
		/// </summary>
		public Fn Attr = (o, d, di, s, si) => {
			d[di[0]] = s[si];
			return 1;
		};
		/// <summary>
		/// Saves list parameter (1 value, append) as string list at list character.
		/// </summary>
		public Fn List = (o, d, di, s, si) => {
			IList<string> l = (l = (IList<string>)d[di[0]]) == null ? (IList<string>)(d[di[0]] = new List<string>()) : l;
			l.Add(s[si]);
			return 1;
		};


		public void PFlag(ref string o) {
			o = "";
		}

		public void PAttr(ref string o, string v) {
			o = v;
		}

		public void PList(List<string> o, string v) {
			o.Add(v);
		}

		public void PSplitList(List<string> o, string v, char[] sep) {
			o.AddRange(v.Split(sep));
		}

		public void PClosedListAttr(List<string> o, string[] v, string begin, string end) {
			o.Clear();
			if (!v[0].StartsWith(begin)) throw new Exception();
			if (v[0].Length != begin.Length) o.Add(v[0].Substring(begin.Length));
			for (int i = 1; i < v.Length; i++) {
				if (v[i].EndsWith(end)) { if (v[i].Length != end.Length) o.Add(v[i].Substring(0, v[i].Length - end.Length)); break; }
				o.Add(v[i]);
			}
		}

		public void PMergeFullAttr(ref string o, string[] v) {
			o = "";
			for (int i = 0; i < v.Length; i++)
				o += v[i];
		}
	}
}
