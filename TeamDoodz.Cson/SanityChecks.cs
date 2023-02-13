using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal static class SanityChecks {
	public static void AssertNotNull([NotNull] object? val) {
		if(val == null) {
			throw new Exception("Unexpected null value. Assertion failed.");
		}
	}
}
