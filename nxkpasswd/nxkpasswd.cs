using System;
using System.Collections.Generic;
using System.IO;

namespace nxkpasswd
{
	public class passwdManager
	{
		public string DictionaryFile { get; set; }
		public int MinimumWordLength { get; set; }
		public int MaximumWordLength { get; set; }

		public int MinimumDictionaryWordCount {get; set; }

		public List<string> validSeparators = new List<string>();
		
		public List<string> validWords = new List<string>();
		
		public passwdManager (int minWordLength, int maxWordLength, string dictionaryFile)
		{
			try
			{
				PopulateNonConstructorDefaults ();
				
				if (!File.Exists(dictionaryFile))
				{
					throw new FileNotFoundException(String.Format ("File {0} was not found.", dictionaryFile));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.Message);
#if DEBUG
				Console.WriteLine (ex.Message + System.Environment.NewLine + ex.StackTrace);
#endif
			}
		}
		
		public passwdManager (int minWordLength, int maxWordLength) : this(minWordLength, maxWordLength, "dict.txt");
		public passwdManager (string dictionaryFile) : this(4, 8, dictionaryFile);
		public passwdManager () : this(4, 8, "dict.txt");
		
		void PopulateNonConstructorDefaults()
		{
			validSeparators.Add (".");
			validSeparators.Add ("*");
			validSeparators.Add ("-");
			validSeparators.Add ("_");
			validSeparators.Add (",");
			validSeparators.Add ("#");
			validSeparators.Add ("|");
			validSeparators.Add ("%");
			validSeparators.Add ("^");
			validSeparators.Add ("&");
			validSeparators.Add ("$");
			validSeparators.Add ("=");
			validSeparators.Add ("+");
			
			MinimumDictionaryWordCount = 100;
		}
		
		List<string> DictionaryLinesToList(string[] array)
		{
			List<string> retVal = new List<string>();
			
			foreach (string s in array)
			{
#if DEBUG
				Console.WriteLine (string.Format ("DEBUG - processing word {0}", s));
#endif
				//skip 'comment' (lines starting with a #) and blank lines in the dictionary file
				//verify that the word meets minimum and maximum length criteria
				if (s.Trim () != "" &&
					!s.StartsWith ("#") &&
					s.Length >= MinimumWordLength &&
					s.Length <= MaximumWordLength)
				{
					retVal.Add (s.Trim ());
				}
			}
			
			return retVal;
		}
		
		public int loadDict()
		{
			
#if DEBUG
			Console.WriteLine (string.Format ("DEBUG - using dictionary file: {0}", DictionaryFile));
#endif
			
			StreamReader sr = new StreamReader(DictionaryFile);
			validWords = DictionaryLinesToList (sr.ReadToEnd().Split(System.Environment.NewLine.ToCharArray()));
			sr.Close ();
			
			// Returning 0 as the error condition to match behaviour in the original Perl version
			// of xkpasswd. Typical .NET convention would be to return 0 on success or use a
			// bool for this method.
			if (!validWords.Count >= MinimumDictionaryWordCount)
			{
				return 0;
			}
			
			return 1;
		}
		
		
	}
}

