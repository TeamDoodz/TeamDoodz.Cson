using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class TypeConverter : CsonConverter<Type> {
	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(reader.CurrentToken.Type != CsonTokenType.TypeValue) {
			throw UnexpectedTokenException.Create(reader.CurrentToken);
		}

		return reader.CurrentToken.ParseToType() ?? throw new CsonConversionException($"Failed to parse \'{reader.ForceGetTokenValueAs<string>()}\' to a type.", expectedType);
	}
}
