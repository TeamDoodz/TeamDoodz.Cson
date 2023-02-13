using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public class UnexpectedTokenException : CsonException {
	public CsonTokenType TokenType { get; }

	public override string ErrorMessage => $"Unexpected token {TokenType}.";

	public UnexpectedTokenException(CsonTokenType token) {
		TokenType = token;
	}
	public static UnexpectedTokenException Create(CsonToken token) {
		return new UnexpectedTokenException(token.Type) { Line = token.StartLine, Column = token.StartColumn };
	}
}
