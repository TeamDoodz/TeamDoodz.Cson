using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class EnumConverter : CsonConverter<Enum> {
	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		// TODO: Support enum names & enum flags

		if(reader.CurrentToken.Type == CsonTokenType.IntValue) {
			return (Enum)Enum.ToObject(expectedType, reader.CurrentToken.GetValueAs<int>());
		}

		throw UnexpectedTokenException.Create(reader.CurrentToken);
	}
}
