using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using TeamDoodz.Cson.Parser;

namespace TeamDoodz.Cson.Example;

internal class Program {
	const string CODE = """
		new TeamDoodz.Cson.Example.ClassA {
		SomeType = typeof(System.Int32),
			SomeText = "Hello, World!"
		}
		""";

	const string CODE_JSON = """
		{
			"Tags": ["1", "2", "3"]
		}
		""";

	static void Main(string[] args) {
		using(CsonTextReader reader = new CsonTextReader(CODE)) {

			CsonSerializer serializer = new();
			ClassA? valA = null;
			Stopwatch sw = Stopwatch.StartNew();

			valA = serializer.Deserialize<ClassA>(reader);

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(valA);

			/*
			Console.WriteLine(reader.CurrentToken);
			while(reader.ReadToken()) {
				Console.WriteLine(reader.CurrentToken);
			}
			*/
		}
		/*
		using(JsonReader jsonReader = new JsonTextReader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(CODE_JSON))))) {
			JsonSerializer jSerializer = new();
			Stopwatch sw = Stopwatch.StartNew();

			ClassA? val = jSerializer.Deserialize<ClassA>(jsonReader);

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
		}
		*/
	}
}
