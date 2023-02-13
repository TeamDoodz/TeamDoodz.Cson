using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public sealed class CsonSerializerSettings {
	public Dictionary<string, Type> TypeAliases { get; set; } = new();
	public List<string> ImplicitUsings { get; set; } = new() { 
		"System",
		"System.IO",
		"System.Collections.Generic",
		"System.Linq",
		"System.Net.Http",
		"System.Threading",
		"System.Threading.Tasks"
	};
}
