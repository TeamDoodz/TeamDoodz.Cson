using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamDoodz.Cson.Example;

public record RecordA(string MyString, bool MyBool, int MyInt) {
	public float MyMutableProperty { get; set; }
}
