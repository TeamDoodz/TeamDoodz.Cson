using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson;

/// <remarks>
///	Inheriting types should call <see cref="ReadToken"/> in their constructor to initialize state.
/// </remarks>
public abstract class CsonReader : IDisposable {
	internal static readonly HashSet<CsonTokenType> numberValueTokens = new() {
		CsonTokenType.IntValue,
		CsonTokenType.LongValue,
		CsonTokenType.UIntValue,
		CsonTokenType.ULongValue,
		CsonTokenType.Float32Value,
		CsonTokenType.Float64Value,
		CsonTokenType.DecimalValue,
	};

	public CsonToken? CurrentToken { get; protected set; }
	public int CurrentLine { get; protected set; } = 1;
	public int CurrentColumn { get; protected set; } = -1;

	public abstract bool ReadToken();

	public bool ReadToken(CsonTokenType expectedTokenType) {
		if(!ReadToken()) {
			return false;
		}
		SanityChecks.AssertNotNull(CurrentToken);
		if(CurrentToken.Type != expectedTokenType) {
			throw UnexpectedTokenException.Create(CurrentToken);
		}
		return true;
	}

	// Names

	public bool ReadIdentifier() => ReadToken(CsonTokenType.Identifier);

	// Basic Values

	public bool ReadStringValue() => ReadToken(CsonTokenType.StringValue);
	public bool ReadIntValue() => ReadToken(CsonTokenType.IntValue);
	public bool ReadLongValue() => ReadToken(CsonTokenType.LongValue);
	public bool ReadUIntValue() => ReadToken(CsonTokenType.UIntValue);
	public bool ReadULongValue() => ReadToken(CsonTokenType.ULongValue);
	public bool ReadFloat32Value() => ReadToken(CsonTokenType.Float32Value);
	public bool ReadFloat64Value() => ReadToken(CsonTokenType.Float64Value);
	public bool ReadDecimalValue() => ReadToken(CsonTokenType.DecimalValue);
	public bool ReadBoolValue() => ReadToken(CsonTokenType.BoolValue);

	public bool ReadNullValue() => ReadToken(CsonTokenType.NullValue);

	public bool ReadNumberValue() {
		if(!ReadToken()) {
			return false;
		}
		SanityChecks.AssertNotNull(CurrentToken);
		if(!numberValueTokens.Contains(CurrentToken.Type)) {
			throw new UnexpectedTokenException(CurrentToken.Type);
		}
		return true;
	}

	// Object Value

	public bool ReadBeginObjectValue() => ReadToken(CsonTokenType.BeginObjectValue);
	public bool ReadEndObjectValue() => ReadToken(CsonTokenType.EndObjectValue);

	// Array Value

	public bool ReadBeginCollectionValue() => ReadToken(CsonTokenType.BeginCollectionValue);
	public bool ReadEndCollectionValue() => ReadToken(CsonTokenType.EndCollectionValue);

	public abstract void Dispose();

	public CsonToken ForceGetToken() {
		if(CurrentToken == null) {
			throw new Exception("Reader token was null.");
		}
		return CurrentToken;
	}
	public T ForceGetTokenValueAs<T>() {
		if(CurrentToken == null) {
			throw new Exception("Reader token was null.");
		}
		return CurrentToken.ForceGetValueAs<T>();
	}

	public void ThrowIfTokenNull() {
		if(CurrentToken == null) {
			throw EndOfCsonStreamException.Create(this);
		}
	}
}
