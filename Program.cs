using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Md5RenamerConsole
{
	static class Program
	{
		readonly static Version version = new Version( 1, 0 );
		readonly static string path = AppDomain.CurrentDomain.BaseDirectory;
		public readonly static string exe = AppDomain.CurrentDomain.FriendlyName;
		readonly static string syntax = string.Format( "{0}{0}{4} [.] [(--dry-run|-dr)]{0}{0}Version {5}{0}{1}{0}{2}{0}{3}{0}{0}",
														Environment.NewLine,
														"- without any arguments, this help is displayed.",
														"- the dot is mandatory to prevent accidentally running of the program.",
														"- '--dry-run' or '-d' just writes logs, but doesn't touch a file (case-insensitive).",
														exe,
														version.ToString() );

		static void Main( string[] args )
		{
			if ( args == null || args.Length < 1 )
			{
				Console.WriteLine( syntax );
				Console.ReadKey();
				Environment.Exit( 0 );
			}

			if ( args[ 0 ] != "." )
			{
				Console.WriteLine( string.Format( "{0}{0}Annotator '.' is missing!{0}{0}", Environment.NewLine ) );
				Console.ReadKey();
				Environment.Exit( 0 );
			}

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			List<string> files = GetFiles();

			if ( files.Count < 1 )
			{
				Console.WriteLine( "No files found. Chickening out..." );
				Console.ReadKey();
				Environment.Exit( 0 );
			}

			Console.WriteLine( string.Format( "{1}{0} files found.{1}Press any key to continue{1}", files.Count, Environment.NewLine ) );
			Console.ReadKey();

			bool simulate = args.Length > 1 && ( args[ 1 ].ToUpper().Equals( "-D" ) || args[ 1 ].ToUpper().Equals( "--DRY-RUN" ) );

			string extension, targetFile;
			WriteNewLine( 2 );

			foreach (string file in files)
			{
				string hash = HashFile(file);

				extension = Path.GetExtension(file);
				targetFile = Path.Combine(Program.path, Path.ChangeExtension(hash, extension));

				if (!File.Exists(targetFile))
				{
					if ( !simulate )
					{
						File.Move( file, targetFile );
					}
					Console.WriteLine( string.Format( "{0} => {1}", Path.GetFileName( file ), Path.GetFileName( targetFile ) ) );
				}
				else
				{
					Console.WriteLine(string.Format("{0} already exists, skipped;", Path.GetFileName(targetFile)));
				}
			}

			stopwatch.Stop();
			Console.WriteLine( "{0}{0}Processed {1} files in {2} seconds.{0}{0}Press any key to exit{0}",
								Environment.NewLine,
								files.Count,
								stopwatch.ElapsedMilliseconds / 1000 );
			Console.ReadKey();
		}

		static List<string> GetFiles()
		{
			string[] array = Directory.GetFiles( path, "*", SearchOption.TopDirectoryOnly );
			List<string> files = new List<string>( array );
			return files.Clean();
		}

		static void WriteNewLine( int times )
		{
			for ( int i = 0; i < times; i++ )
			{
				Console.WriteLine( Environment.NewLine );
			}
		}

		/// <summary>
		/// HashFile
		/// </summary>
		/// <param name="path">The full path to the file.</param>
		/// <returns>A string with the MD5 sum of the file.</returns>
		static string HashFile( string path )
		{
			StringBuilder sb = new StringBuilder();
			MD5 md5Hasher = MD5.Create();

			using ( FileStream fs = File.OpenRead( path ) )
			{
				foreach ( Byte b in md5Hasher.ComputeHash( fs ) )
				{
					sb.Append( b.ToString( "X2" ) );
				}
			}

			return sb.ToString();
		}

	}
}
