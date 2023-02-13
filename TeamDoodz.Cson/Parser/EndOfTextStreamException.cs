using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Parser;

public class EndOfTextStreamException : CsonException {
	public override string ErrorMessage => "Unexpected end of text stream.";
}
