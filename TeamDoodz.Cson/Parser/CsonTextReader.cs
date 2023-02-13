using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Parser;

public partial class CsonTextReader : CsonReader {
	static readonly HashSet<char> digitChars = new() {
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
	};
	static readonly HashSet<char> seperatorChars = new() {
		',', '=', '{', '(', ')'
	};

	private enum State {
		Start,
		BeginObject,
		BeginGenerics,
		Generics,
		Property,
		PropertyValue,
		PostProperty,
		Collection,
		PostCollectionElement
	}

	private enum Nestable {
		Object,
		Collection
	}

	private readonly Stack<Nestable> nestingStack = new();

	private State currentState = State.Start;

	private TextReader reader;

	private char currentChar;
	private bool queueNewLine;

	private int currentStartLine;
	private int currentStartColumn;

	public CsonTextReader(TextReader reader) {
		this.reader = reader;

		ReadToken();
	}
	public CsonTextReader(string text) : this(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)))) {

	}

	private bool ReadChar() {
		char[] buffer = new char[1];
		if(reader.ReadBlock(buffer, 0, 1) == 0) {
			currentChar = '\0';
			return false;
		}
		currentChar = buffer[0];
		CurrentColumn++;
		if(queueNewLine) {
			queueNewLine = false;
			CurrentLine++;
			CurrentColumn = 0;
		}
		if(currentChar == '\n') {
			queueNewLine = true;
		}
		return true;
	}

	public override void Dispose() {
		reader.Dispose();
		GC.SuppressFinalize(this);
	}

	public override bool ReadToken() {

		// If still at start state and previous token is not null, that means ParseValue read a literal value at the start of the stream
		// Meaning the document is over
		if(currentState == State.Start && CurrentToken != null) {
			return false;
		}

		switch(currentState) {
			case State.Start:
				ReadChar();
				return ParseValue();
			case State.BeginObject:
				if(!ParseIdentifier()) return false;
				MatchString("{");
				currentState = State.Property;
				return true;
			case State.BeginGenerics:
				return ParseBeginGenerics();
			case State.Property:
				if(!ParseIdentifier()) return false;
				MatchString("=");
				currentState = State.PropertyValue;
				return true;
			case State.PropertyValue:
			case State.Collection:
				if(!ParseValue()) return false;
				//if(currentState == State.PropertyValue) currentState = State.PostProperty; // if ParseValue doesn't change the state, change it to PostProperty
				return true;
			case State.PostProperty:
				return ParsePostProperty();
			case State.PostCollectionElement:
				return ParsePostCollectionElement();
		}

		return false;
	}

	private bool ParseBeginGenerics() {
		ReadUntilNotWhitespace();

		if(currentChar == '{') {
			currentState = State.BeginObject;
		}
		if(currentChar == '<') {
			currentState = State.Generics;
		}

		ReadChar();
		return true;
	}

	private void ReadOnceThenReadUntilNotWhitespace() {
		do {
			if(!ReadChar()) {
				throw CreateEndOfTextStreamException();
			}
		} while(char.IsWhiteSpace(currentChar));
	}

	private void ReadUntilNotWhitespace() {
		while(char.IsWhiteSpace(currentChar)) {
			if(!ReadChar()) {
				throw CreateEndOfTextStreamException();
			}
		}
	}

	private bool ParsePostCollectionElement() {
		while(char.IsWhiteSpace(currentChar)) {
			if(!ReadChar()) {
				return false;
			}
		}
		SetStartOfTokenHere();

		switch(currentChar) {
			case ',':
				currentState = State.Collection;
				ReadChar();
				return ReadToken();
			case ']':
				SetToken(CsonTokenType.EndCollectionValue, "]");
				ReadChar();
				PopNestable();
				return true;
			case '\0':
				return false;
		}

		throw CreateUnexpectedCharacterException(currentChar);
	}

	private void PopNestable() {
		nestingStack.Pop();
		if(nestingStack.Count == 0) {
			return;
		}
		Nestable n = nestingStack.Peek();
		currentState = n switch {
			Nestable.Collection => State.PostCollectionElement,
			Nestable.Object => State.PostProperty,
			_ => (State)(-1)
		};
	}

	private void SetPostForCurrentNestable() {
		if(nestingStack.Count == 0) {
			return;
		}
		Nestable n = nestingStack.Peek();
		if(n == Nestable.Object) {
			currentState = State.PostProperty;
		} else {
			currentState = State.PostCollectionElement;
		}
	}

	private bool ParsePostProperty() {
		while(char.IsWhiteSpace(currentChar)) {
			if(!ReadChar()) {
				return false;
			}
		}
		SetStartOfTokenHere();

		switch(currentChar) {
			case ',':
				currentState = State.Property;
				ReadChar();
				return ReadToken();
			case '}':
				SetToken(CsonTokenType.EndObjectValue, "}");
				ReadChar();
				PopNestable();
				return true;
			case '\0':
				return false;
		}

		throw CreateUnexpectedCharacterException(currentChar);
	}

	private bool ParseIdentifier(bool dontReadAhead = false, string startingString = "") {
		if(!dontReadAhead) {
			ReadOnceThenReadUntilNotWhitespace();
		}
		SetStartOfTokenHere();

		bool firstChar = true;
		StringBuilder sb = new(startingString);

		while(!IsSeperator(currentChar)) {
			Regex regex = firstChar ? GetIDFirstCharRegex() : GetIDCharRegex();
			firstChar = false;

			if(!regex.IsMatch(currentChar.ToString())) {
				throw CreateUnexpectedCharacterException(currentChar);
			}

			sb.Append(currentChar);

			if(!ReadChar()) {
				break;
			}
		}

		SetToken(CsonTokenType.Identifier, sb.ToString());
		return true;
	}

	private bool ParseValue() {
		ReadUntilNotWhitespace();
		SetStartOfTokenHere();

		switch(currentChar) {
			case '"':
				ParseString();
				SetPostForCurrentNestable();
				return true;
			case 't':
				if(!ReadChar()) {
					SetPostForCurrentNestable();
					return ParseIdentifier(dontReadAhead: false, startingString: "t");
				}
				if(currentChar == 'r') {
					ParseTrue();
					SetPostForCurrentNestable();
					return true;
				} else if(currentChar == 'y') {
					ParseTypeValue();
					SetPostForCurrentNestable();
					return true;
				}

				SetPostForCurrentNestable();
				return ParseIdentifier(dontReadAhead: false, startingString: "t" + currentChar);
			case 'f':
				ParseFalse();
				SetPostForCurrentNestable();
				return true;
			case 'n':
				if(!ReadChar()) {
					SetPostForCurrentNestable();
					return ParseIdentifier(dontReadAhead: false, startingString: "n");
				}
				if(currentChar == 'e') {
					ParseBeginObject();
					return true;
				} else if(currentChar == 'u') {
					ParseNull();
					SetPostForCurrentNestable();
					return true;
				}

				SetPostForCurrentNestable();
				return ParseIdentifier(dontReadAhead: false, startingString: "n" + currentChar);
			case '[':
				ReadChar();
				SetToken(CsonTokenType.BeginCollectionValue, "[");
				nestingStack.Push(Nestable.Collection);
				currentState = State.Collection;
				return true;
			default:
				if(digitChars.Contains(currentChar) || currentChar == '-' || currentChar == '.') {
					// number
					ParseNumber();
					SetPostForCurrentNestable();
					return true; // ParseNumber already moves the current character after the parsed token
				}

				return ParseIdentifier(dontReadAhead: true);

				//throw CreateUnexpectedCharacterException(currentChar);
		}
	}

	private void ParseTypeValue() {
		SetStartOfTokenHere();

		MatchString("ypeof");
		MatchString("(");
		ReadUntilNotWhitespace();

		StringBuilder typeName = new();

		while(!IsSeperator(currentChar)) {
			typeName.Append(currentChar);
			if(!ReadChar()) {
				throw CreateEndOfTextStreamException();
			}
		}
		while(true) {
			if(char.IsWhiteSpace(currentChar)) {
				if(!ReadChar()) {
					throw CreateEndOfTextStreamException();
				}
				continue;
			}
			if(currentChar == ')') {
				ReadChar();
				break;
			}
			throw CreateUnexpectedCharacterException(currentChar);
		}

		SetToken(CsonTokenType.TypeValue, typeName.ToString());
	}

	private void MatchString(string str) {
		ReadUntilNotWhitespace();

		char[] chars = str.ToCharArray();
		if(chars[0] != currentChar) {
			throw CreateUnexpectedCharacterException(currentChar);
		}
		for(int i=1; i < chars.Length; i++) {
			if(!ReadChar()) {
				throw CreateEndOfTextStreamException();
			}
			if(chars[i] != currentChar) {
				throw CreateUnexpectedCharacterException(currentChar);
			}
		}
		ReadChar();
	}

	private void ParseBeginObject() {
		MatchString("ew");
		currentState = State.BeginObject;
		SetToken(CsonTokenType.BeginObjectValue, "new");
		nestingStack.Push(Nestable.Object);
	}

	private void ParseNull() {
		MatchString("ull");
		SetToken(CsonTokenType.NullValue, "null");
	}

	private void ParseNumber() {
		StringBuilder numberSoFar = new();

		bool isHandlingDigits = true;
		bool usedDot = false;
		bool firstChar = true;
		bool isUnsigned = false;

		HandleChar(currentChar);
		while(ReadChar()) {
			if(HandleChar(currentChar)) {
				return;
			}
		}
		if(HandleChar('\0')) {
			return;
		}

		throw CreateEndOfTextStreamException();

		bool HandleChar(char c) {
			if(isHandlingDigits) {
				bool isDigit = digitChars.Contains(c);
				bool isDot = c == '.' && !usedDot;
				bool isMinus = c == '-' && firstChar;

				if(isDot) {
					usedDot = true;
				}

				if(isDigit || isDot || isMinus) {
					numberSoFar.Append(c);
					return false;
				}
			}

			if(IsSeperator(c)) {
				if(isUnsigned && !usedDot) {
					// Unsigned int
					FinishUInt();
				}

				// Number doesn't have a suffix, so either int or double
				if(usedDot) {
					FinishFloat64();
					return true;
				} else {
					FinishInt();
					return true;
				}
			}

			if((c == 'u' || c == 'U') && !usedDot) {
				isUnsigned = true;
				isHandlingDigits = false;
				return false;
			}

			if((c == 'f' || c == 'F') && !isUnsigned) {
				// Float32
				FinishFloat32();
				ReadChar();
				return true;
			}
			if((c == 'd' || c == 'D') && !isUnsigned) {
				// Float64
				FinishFloat64();
				ReadChar();
				return true;
			}

			if((c == 'l' || c == 'L') && !usedDot) {
				// Long (wow who would have guessed)
				if(isUnsigned) {
					FinishULong();
					ReadChar();
					return true;
				} else {
					FinishLong();
					ReadChar();
					return true;
				}
			}

			throw CreateUnexpectedCharacterException(c);
		}

		void FinishInt() {
			SetToken(CsonTokenType.IntValue, int.Parse(numberSoFar.ToString()));
		}
		void FinishLong() {
			SetToken(CsonTokenType.LongValue, long.Parse(numberSoFar.ToString()));
		}
		void FinishFloat32() {
			SetToken(CsonTokenType.Float32Value, float.Parse(numberSoFar.ToString()));
		}
		void FinishFloat64() {
			SetToken(CsonTokenType.Float64Value, double.Parse(numberSoFar.ToString()));
		}
		void FinishUInt() {
			SetToken(CsonTokenType.UIntValue, uint.Parse(numberSoFar.ToString()));
		}
		void FinishULong() {
			SetToken(CsonTokenType.LongValue, ulong.Parse(numberSoFar.ToString()));
		}
	}

	private void ParseTrue() {
		MatchString("rue");
		SetToken(CsonTokenType.BoolValue, true);
	}
	private void ParseFalse() {
		MatchString("false");
		SetToken(CsonTokenType.BoolValue, false);
	}

	private void ParseString() {
		StringBuilder stringContent = new();

		bool escapeNext = false;

		while(true) {
			if(!ReadChar()) {
				throw CreateEndOfTextStreamException();
			}
			if(currentChar == '\\') {
				escapeNext = true;
				continue;
			}
			if(!escapeNext) {
				if(currentChar == '"') {
					break;
				}
			}
			stringContent.Append(currentChar);
			escapeNext = false;
		}

		ReadChar();

		SetToken(CsonTokenType.StringValue, stringContent.ToString());
	}

	private void SetToken(CsonTokenType type, object? value) {
		CurrentToken = new CsonToken(type) { StartLine = currentStartLine, StartColumn = currentStartColumn, Value = value };
	}
	private void SetStartOfTokenHere() {
		currentStartLine = CurrentLine;
		currentStartColumn = CurrentColumn;
	}

	private static bool IsSeperator(char c) {
		return char.IsWhiteSpace(c) || c == '\0' || seperatorChars.Contains(c);
	}

	private EndOfTextStreamException CreateEndOfTextStreamException() {
		return new EndOfTextStreamException() { Line = CurrentLine, Column = CurrentColumn };
	}

	private UnexpectedCharacterException CreateUnexpectedCharacterException(char c) {
		return new UnexpectedCharacterException(c) { Line = CurrentLine, Column = CurrentColumn };
	}

	[GeneratedRegex(@"[a-zA-Z_]")]
	private static partial Regex GetIDFirstCharRegex();
	[GeneratedRegex(@"[a-zA-Z_0-9.-]")]
	private static partial Regex GetIDCharRegex();
}
