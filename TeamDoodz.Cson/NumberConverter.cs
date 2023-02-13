using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class NumberConverter : CsonConverter {
	static readonly HashSet<Type> allowedTypes = new HashSet<Type>() {
		typeof(int),
		typeof(long),
		typeof(byte),
		typeof(uint),
		typeof(ulong),
		typeof(sbyte),
		typeof(float),
		typeof(double),
		typeof(decimal),
	};

	public override bool CanConvert(Type type) {
		return allowedTypes.Contains(type);
	}

	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(!reader.CurrentToken.IsNumber()) {
			throw UnexpectedTokenException.Create(reader.CurrentToken);
		}

		return reader.CurrentToken.GetValueAs(expectedType);
	}
}
