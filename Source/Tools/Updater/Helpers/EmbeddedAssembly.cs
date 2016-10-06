using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

//Source: http://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource
namespace mxd.GZDBUpdater
{
	public static class EmbeddedAssembly
	{
		private static Dictionary<string, Assembly> dic = new Dictionary<string, Assembly>();

		public static void Load(string embeddedResource, string filename)
		{
			byte[] ba;
			Assembly asm;
			Assembly curAsm = Assembly.GetExecutingAssembly();

			using(Stream stm = curAsm.GetManifestResourceStream(embeddedResource))
			{
				// Either the file is not existed or it is not mark as embedded resource
				if(stm == null) throw new Exception(embeddedResource + " is not found in Embedded Resources.");

				// Get byte[] from the file from embedded resource
				ba = new byte[(int)stm.Length];
				stm.Read(ba, 0, (int)stm.Length);
				try
				{
					asm = Assembly.Load(ba);

					// Add the assembly/dll into dictionary
					dic.Add(asm.FullName, asm);
					return;
				}
				catch
				{
					// Purposely do nothing
					// Unmanaged dll or assembly cannot be loaded directly from byte[]
					// Let the process fall through for next part
				}
			}

			bool fileOk = false;
			string tempFile;

			using(SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
			{
				string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);
				tempFile = Path.GetTempPath() + filename;

				if(File.Exists(tempFile))
				{
					byte[] bb = File.ReadAllBytes(tempFile);
					string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);
					fileOk = (fileHash == fileHash2);
				}
			}

			if(!fileOk)
			{
				File.WriteAllBytes(tempFile, ba);
			}

			asm = Assembly.LoadFile(tempFile);
			dic.Add(asm.FullName, asm);
		}

		public static Assembly Get(string assemblyFullName)
		{
			if(dic == null || dic.Count == 0) return null;
			return (dic.ContainsKey(assemblyFullName) ? dic[assemblyFullName] : null);
		}
	}
}
