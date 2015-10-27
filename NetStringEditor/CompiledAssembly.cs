using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using PropertyChanged;

namespace NetStringEditor
{
	[ImplementPropertyChanged]
	public class CompiledAssembly
	{
		public AssemblyDefinition Assembly { get; }

		public ObservableCollection<CompiledString> Strings { get; } = new ObservableCollection<CompiledString>();

		private CompiledAssembly(AssemblyDefinition assembly)
		{
			Assembly = assembly;
		}

		public void Save(Stream dest)
		{
			Assembly.Write(dest);
			foreach (var str in Strings)
				str.Commit();
		}

		public static CompiledAssembly Load(Stream stream)
		{
			var asm = new CompiledAssembly(AssemblyDefinition.ReadAssembly(stream));

			foreach (var module in asm.Assembly.Modules)
			{
				foreach (var type in module.Types)
				{
					foreach (var method in type.Methods.Where(m => m.Body != null))
					{
						foreach (var inst in method.Body.Instructions)
						{
							if (inst.OpCode == OpCodes.Ldstr)
							{
								Debug.WriteLine($"{type.Name}.{method.Name}: {inst.Operand}");
								asm.Strings.Add(new CompiledString(method, inst));
							}
						}
					}
				}
			}

			return asm;
		}
	}
}
