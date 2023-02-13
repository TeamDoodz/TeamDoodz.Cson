using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

// TODO: Support read-only Collection properties by calling the Add method instead of setting a new value

internal sealed class CollectionConverter : CsonConverter {
	public override bool CanConvert(Type type) {
		return type.GetInterface("ICollection`1") != null;
	}

	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(reader.CurrentToken.Type != CsonTokenType.BeginCollectionValue) {
			throw UnexpectedTokenException.Create(reader.CurrentToken);
		}

		// read first token of collection
		if(!reader.ReadToken()) {
			throw EndOfCsonStreamException.Create(reader);
		}

		if(expectedType.GetConstructor(Array.Empty<Type>()) is not ConstructorInfo constr) {
			throw new CsonConversionException($"Type {expectedType} cannot be instantiated as it does not implement a public parameterless constructor.", expectedType) { Line = reader.CurrentLine, Column = reader.CurrentColumn };
		}

		Type elementType = (expectedType.GetInterface("ICollection`1") ?? throw new Exception("Type does not implement ICollection<T>.")).GenericTypeArguments[0];
		MethodInfo addMethod = typeof(ICollection<>).MakeGenericType(elementType).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance) ?? throw new Exception("Could not find Add method on type ICollection<T>");

		object retVal = constr.Invoke(null);

		while(reader.CurrentToken.Type != CsonTokenType.EndCollectionValue) {
			addMethod.Invoke(retVal, new object?[] { serializer.Deserialize(elementType, reader) });
			if(!reader.ReadToken()) {
				throw EndOfCsonStreamException.Create(reader);
			}
		}

		return retVal;
	}
}
