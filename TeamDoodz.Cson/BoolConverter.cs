using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class BoolConverter : CsonConverter<bool> {
	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(reader.CurrentToken.Type != CsonTokenType.BoolValue) {
			throw UnexpectedTokenException.Create(reader.CurrentToken);
		}

		return reader.CurrentToken.GetValueAs<bool>();
	}
}
