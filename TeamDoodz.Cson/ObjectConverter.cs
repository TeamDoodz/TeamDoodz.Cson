using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

internal sealed class ObjectConverter : CsonConverter {
	public override bool CanConvert(Type type) {
		return !type.IsGenericType && !typeof(MulticastDelegate).IsAssignableFrom(type.BaseType);
	}

	public override object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer) {
		reader.ThrowIfTokenNull();
		SanityChecks.AssertNotNull(reader.CurrentToken);

		if(reader.CurrentToken.Type == CsonTokenType.NullValue) {
			return null;
		}

		if(reader.CurrentToken.Type != CsonTokenType.BeginObjectValue) {
			throw UnexpectedTokenException.Create(reader.CurrentToken);
		}

		if(!reader.ReadIdentifier()) {
			throw EndOfCsonStreamException.Create(reader);
		}
		Type typeToCreate = reader.ForceGetToken().ParseToType() ?? throw new CsonConversionException($"Failed to parse \'{reader.ForceGetTokenValueAs<string>()}\' to a type.", expectedType);
		if(!typeToCreate.IsAssignableTo(expectedType)) {
			throw new CsonConversionException($"Type {typeToCreate} cannot be instantiated as it is not assignable to the converting type.", expectedType) { Line = reader.CurrentLine, Column = reader.CurrentColumn };
		}
		if(typeToCreate.GetConstructor(Array.Empty<Type>()) is not ConstructorInfo constr) {
			throw new CsonConversionException($"Type {typeToCreate} cannot be instantiated as it does not implement a public parameterless constructor.", expectedType) { Line = reader.CurrentLine, Column = reader.CurrentColumn };
		}

		object retVal = constr.Invoke(null);

		while(true) {
			if(!reader.ReadToken()) {
				throw EndOfCsonStreamException.Create(reader);
			}

			SanityChecks.AssertNotNull(reader.CurrentToken);

			if(reader.CurrentToken.Type == CsonTokenType.EndObjectValue) {
				break;
			}

			if(reader.CurrentToken.Type != CsonTokenType.Identifier) {
				throw UnexpectedTokenException.Create(reader.CurrentToken);
			}

			//TODO: Support public fields
			PropertyInfo prop = reader.CurrentToken.ParseToProperty(typeToCreate) ?? throw new CsonConversionException($"Failed to parse \'{reader.ForceGetTokenValueAs<string>()}\' to a property.", expectedType) { Line = reader.CurrentLine, Column = reader.CurrentColumn };

			// Read ahead
			if(!reader.ReadToken()) {
				throw EndOfCsonStreamException.Create(reader);
			}

			object? val = serializer.Deserialize(prop.PropertyType, reader);

			prop.SetValue(retVal, val);
		}

		return retVal;
	}
}