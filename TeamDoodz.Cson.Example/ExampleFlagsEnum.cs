using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Example;

[Flags]
public enum ExampleFlagsEnum {
	None = 0,
	ChoiceA = 1 << 0,
	ChoiceB = 1 << 1,
	ChoiceC = 1 << 3,
	All = ChoiceA | ChoiceB | ChoiceC
}
