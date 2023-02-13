using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public abstract class CsonException : Exception {
	public abstract string ErrorMessage { get; }
	public int Line { get; init; }
	public int Column { get; init; }

	public override string Message => $"({Line}, {Column}): {ErrorMessage}";
}
