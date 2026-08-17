//
// The QA Department Website Plugin - Extensions to Pretzel.
// Copyright (C) 2026 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System.Text.RegularExpressions;
using Pretzel.Logic.Templating.Context;

namespace SitePlugin
{
    public static class PageExtensions
    {
        // ---------------- Fields ----------------
        
        private static readonly Regex comicIdRegex = new Regex( 
            @"^\d+-\d+-\d+-(?<comicId>\d+)",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        // ---------------- Methods ----------------

        public static bool IsComicPage( this Page page )
        {
            return page.Bag.ContainsKey( "comic" );
        }

        public static int GetComicId( this Page page )
        {
            string fileName = Path.GetFileName( page.File );

            Match match = comicIdRegex.Match( fileName );
            if( match.Success == false )
            {
                throw new ArgumentException( 
                    $"Could not find comic id in file name: {fileName}",
                    nameof( page )
                );
            }

            return int.Parse( match.Groups["comicId"].Value );
        }

        public static string GetPageTranscript( this Page page )
        {
            string? directory = Path.GetDirectoryName( page.File );
            if( string.IsNullOrWhiteSpace( directory ) )
            {
                throw new InvalidOperationException( $"Could not get directory of {page.File}" );
            }

            string targetPath = Path.Combine(
                directory,
                "..",
                "_transcripts",
                Path.GetFileName( page.File )
            );

            Console.WriteLine( "Target: " + targetPath );

            if( Path.Exists( targetPath ) == false )
            {
                throw new ArgumentException(
                    $"Can not find transcript for page: {page.File}"
                );
            }

            var markdownEngine = new QaMarkdownEngine();
            string fileContents = File.ReadAllText( targetPath );

            return markdownEngine.Convert( fileContents );
        }
    }
}
