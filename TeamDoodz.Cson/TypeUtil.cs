using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal static class TypeUtil {
	public static Type? TypeFromName(string name) {
		foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse()) {
			var tt = assembly.GetType(name);
			if(tt != null) {
				return tt;
			}
		}

		return null;
	}
}
