using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Example;

public class ClassA {
	public string SomeText { get; set; } = "Just Some Text";

	public bool Enabled { get; init; } = false;

	public ExampleEnum SomeEnum { get; init; } = ExampleEnum.Value3;

	public ClassA? Child { get; set; } = null;

	public int? MaybeInt { get; set; } = 420;

	public List<string> Tags { get; } = new();

	public Type SomeType { get; set; }
}
