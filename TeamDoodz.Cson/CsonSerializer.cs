using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public class CsonSerializer {
	static readonly List<CsonConverter> builtinConverters = new() { 
		new StringConverter(),
		new NullableValueTypeConverter(),
		new NumberConverter(),
		new BoolConverter(),
		new TypeConverter(),
		new EnumConverter(),
		new CollectionConverter(),
		new ObjectConverter()
	};

	public object? Deserialize(Type expectedType, CsonReader reader) {
		foreach(CsonConverter converter in builtinConverters) {
			if(converter.CanConvert(expectedType)) {
				return converter.ReadCson(expectedType, reader, this);
			}
		}
		throw new InvalidOperationException($"Could not find converter for type {expectedType}.");
	}

	public T? Deserialize<T>(CsonReader reader) {
		object? retVal = Deserialize(typeof(T), reader);
		return (T?)retVal;
	}
}
