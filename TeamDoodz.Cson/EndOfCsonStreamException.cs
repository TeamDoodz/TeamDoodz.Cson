using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public class EndOfCsonStreamException : CsonException {
	public override string ErrorMessage => "Unexpected end of Cson stream.";

	public static EndOfCsonStreamException Create(CsonReader reader) {
		return new EndOfCsonStreamException() { Line = reader.CurrentLine, Column = reader.CurrentColumn };
	}
}
