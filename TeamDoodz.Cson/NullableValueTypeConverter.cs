using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class NullableValueTypeConverter : CsonConverter {
	public override bool CanConvert(Type type) {
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
	}

	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(reader.CurrentToken.Type == CsonTokenType.NullValue) {
			return null;
		}

		Type t = expectedType.GetGenericArguments()[0];
		return serializer.Deserialize(t, reader);
	}
}
