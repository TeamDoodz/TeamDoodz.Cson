using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Parser;

public class UnknownTokenException : CsonException {
	public override string ErrorMessage => $"Could not resolve \'{TokenText}\' to a token.{GetExpectedTypeListString()}";
	public string TokenText { get; }
	public IEnumerable<CsonTokenType> ExpectedTypes { get; init; } = Array.Empty<CsonTokenType>();

	private string GetExpectedTypeListString() {
		string list = string.Join(", ", ExpectedTypes);
		if(list.Length > 0) {
			return " Expected tokens: " + list;
		} else {
			return "";
		}
	}

	public UnknownTokenException(string tokenText) {
		TokenText = tokenText;
	}
}
