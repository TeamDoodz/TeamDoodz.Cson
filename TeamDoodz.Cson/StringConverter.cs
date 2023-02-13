using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class StringConverter : CsonConverter<string> {
	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(reader.CurrentToken.Type != CsonTokenType.StringValue) {
			throw UnexpectedTokenException.Create(reader.CurrentToken);
		}

		return reader.CurrentToken.GetValueAs<string>();
	}
}
