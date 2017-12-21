using System.Collections.Generic;
using System.IO;

namespace Md5RenamerConsole
{
	public static class Extensions
	{
		public static List<string> Clean( this List<string> list )
		{
			string testAgainst = Path.GetFileNameWithoutExtension( Program.exe ).ToUpper();
			string testee = string.Empty;

			for ( int i = list.Count - 1; i >= 0; i-- )
			{
				testee = Path.GetFileName( list[ i ] ).ToUpper();
				if ( testee.StartsWith( testAgainst ) ||
					 testee.Equals( "DESKTOP.INI" ) )
				{
					list.Remove( list[ i ] );
				}
			}

			return list;
		}
	}
}
