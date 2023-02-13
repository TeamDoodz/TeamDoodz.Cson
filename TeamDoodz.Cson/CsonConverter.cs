using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public abstract class CsonConverter {
	public abstract bool CanConvert(Type type);

	/// <summary>
	/// Reads Cson data and converts it into an object.
	/// </summary>
	/// <param name="expectedType">The type to convert to.</param>
	/// <param name="reader">The reader that will be used uring conversion. At the start of this method, the reader should be at the first token of the data; at the end of the method, the reader will be at the last token of the data.</param>
	/// <param name="serializer">A <see cref="CsonSerializer"/> that the converter may use for complex conversion.</param>
	/// <returns></returns>
	public abstract object? ReadCson(Type expectedType, CsonReader reader, CsonSerializer serializer);
}

public abstract class CsonConverter<T> : CsonConverter {
	public override bool CanConvert(Type type) {
		return typeof(T).IsAssignableFrom(type);
	}
}
