using System;
using System.Linq;
using System.Reflection;
using TeamDoodz.Cson.Parser;

namespace TeamDoodz.Cson;

public sealed class CsonToken {
	public CsonTokenType Type { get; }
	public object? Value { get; init; }
	public int StartLine { get; init; }
	public int StartColumn { get; init; }

	public CsonToken(CsonTokenType type) {
		Type = type;
	}

	public object? GetValueAs(Type type) {
		if(Value == null) {
			return default;
		}
		if(Value.GetType().IsAssignableTo(type)) {
			return Value;
		}
		throw new Exception($"Type {Value.GetType()} is not assignable to type {type}.");
	}

	public T? GetValueAs<T>() {
		return (T?)GetValueAs(typeof(T));
	}

	public T ForceGetValueAs<T>() {
		return GetValueAs<T>() ?? throw new Exception($"Token value was null.");
	}

	public Type? ParseToType() {
		if(GetValueAs<string>() is not string text) {
			throw new Exception("Token value is null.");
		}

		return TypeUtil.TypeFromName(text);
	}

	public PropertyInfo? ParseToProperty(Type type) {
		if(GetValueAs<string>() is not string text) {
			throw new Exception("Token value is null.");
		}

		return type.GetProperty(text);
	}

	public bool IsNumber() {
		return CsonReader.numberValueTokens.Contains(Type);
	}

	public override string ToString() {
		return $"[{Type}: \'{Value ?? "<null>"}\' @({StartLine}, {StartColumn})]";
	}
}
