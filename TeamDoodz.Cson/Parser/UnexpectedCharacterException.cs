using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Parser;

public class UnexpectedCharacterException : CsonException {
	public override string ErrorMessage => $"Unexpected character \'{Character}\'.";

	public char Character { get; }

	public UnexpectedCharacterException(char c) {
		Character = c;
	}
}
