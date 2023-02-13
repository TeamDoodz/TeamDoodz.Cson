using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

public enum CsonTokenType {
    // Names

    /// <summary>
    /// Token that represents an identifier.
    /// </summary>
    Identifier,

	// Generics

	/// <summary>
	/// Token that starts generic arguments.
	/// </summary>
	BeginGenericArgs,
	/// <summary>
	/// Token that starts generic arguments.
	/// </summary>
	EndGenericArgs,

    // Basic Values

    /// <summary>
    /// Token that represents a string value.
    /// </summary>
    StringValue,
	/// <summary>
	/// Token that represents an int value.
	/// </summary>
	IntValue,
	/// <summary>
	/// Token that represents a long value.
	/// </summary>
	LongValue,
	/// <summary>
	/// Token that represents an unsigned int value.
	/// </summary>
	UIntValue,
	/// <summary>
	/// Token that represents an unsigned long value.
	/// </summary>
	ULongValue,
	/// <summary>
	/// Token that represents a single-precision float value.
	/// </summary>
	Float32Value,
	/// <summary>
	/// Token that represents a double-precision float value.
	/// </summary>
	Float64Value,
	/// <summary>
	/// Token that represents a decimal value.
	/// </summary>
	DecimalValue,
	/// <summary>
	/// Token that represents a boolean value.
	/// </summary>
	BoolValue,
	/// <summary>
	/// Token that represents a type value.
	/// </summary>
	TypeValue,

	/// <summary>
	/// Token that represents a null value.
	/// </summary>
	NullValue,

	// Object Value

	/// <summary>
	/// Token that starts an object value.
	/// </summary>
	BeginObjectValue,
	/// <summary>
	/// Token that ends an object value.
	/// </summary>
	EndObjectValue,

	// Array Value

	/// <summary>
	/// Token that starts a collection value.
	/// </summary>
	BeginCollectionValue,
	/// <summary>
	/// Token that ends a collection value.
	/// </summary>
	EndCollectionValue,
}
