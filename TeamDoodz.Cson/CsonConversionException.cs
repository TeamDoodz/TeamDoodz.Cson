using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public class CsonConversionException : CsonException {
	public override string ErrorMessage => $"Failed to convert type {ConvertingType}: {ConverterMessage}";

	public string ConverterMessage { get; }
	public Type ConvertingType { get; }

	public CsonConversionException(string msg, Type type) {
		ConverterMessage = msg;
		ConvertingType = type;
	}
}
