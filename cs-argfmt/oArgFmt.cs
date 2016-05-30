using System;
using System.Collections.Generic;

namespace orez.argfmt {
	class oArgFmt {

		// types
		public delegate int Fn(oArgFmt o, IList<object> dst, int di, IList<string> src, int si);

		// data
		public string LongPrefix = "--";
		public string ShortPrefix = "-";
		public IList<IList<object>> Fmt;

		public oArgFmt() {
		}

		public void Get(IList<object> m, IList<string> args) {
			for(int i = 0; i < args.Count; i++)
				i += OnArg(this, m, 0, args, i+1);
		}


		// delegates
		/// <summary>
		/// Saves parameter based on its type (<code>LongPrefix</code>, <code>ShortPrefix</code>).
		/// </summary>
		public Fn OnArg = (o, d, di, s, si) => {
			int t = 0; string p = s[si];
			if(p.StartsWith(o.LongPrefix)) return o.Param(o, d, 1, s, si);
			else if(!p.StartsWith(o.ShortPrefix)) return o.Param(o, d, 0, s, si);
			for(int i = o.ShortPrefix.Length; i < p.Length; i++)
				t += o.Param(o, d, 1, s, si);
			return t;
		};
		/// <summary>
		/// Saves parameter based on format-table <code>Fmt</code>.
		/// </summary>
		public Fn Param = (o, d, di, s, si) => {
			bool v = di == 0; string k = v ? "" : s[si];
			for(int i = 0; i < o.Fmt.Count; i++) {
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
