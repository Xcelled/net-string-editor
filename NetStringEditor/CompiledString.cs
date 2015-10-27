using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using PropertyChanged;

namespace NetStringEditor
{
	[ImplementPropertyChanged]
	public class CompiledString
	{
		private static int PaddingLen = int.MaxValue.ToString(CultureInfo.InvariantCulture).Length;

		public MethodDefinition Method { get; }
		public Instruction Instruction { get; }
		
		[DependsOn(nameof(Method))]
		public string Location { get { return $"{Method.DeclaringType.Name}.{Method.Name}"; } }

		[DependsOn(nameof(Location))]
		public string SortableLocation
		{
			get { return Location + Instruction.Offset.ToString(CultureInfo.InvariantCulture).PadLeft(PaddingLen, '0'); }
		}

		[DependsOn(nameof(Instruction))]
		public string Value { get { return Instruction.Operand.ToString(); } set { Instruction.Operand = value; } }
		[DependsOn(nameof(Value))]
		public string HexValue { get { return BitConverter.ToString(Encoding.Unicode.GetBytes(Value)).Replace('-', ' '); } }

		public string OriginalValue { get; private set; }
		[DependsOn(nameof(OriginalValue), nameof(Value))]
		public bool IsDirty { get { return OriginalValue != Value; } }

		public CompiledString(MethodDefinition method, Instruction instruction)
		{
			Method = method;
			Instruction = instruction;

			OriginalValue = Value;
		}

		/// <summary>
		/// Resets this instance to the value it had last time Commit was called.
		/// </summary>
		public void Reset()
		{
			Value = OriginalValue;
		}

		/// <summary>
		/// Commits this instance.
		/// </summary>
		public void Commit()
		{
			OriginalValue = Value;
		}
	}
}
