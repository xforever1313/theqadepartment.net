//
// The QA Department Web Comic DevOps - Build Tools.
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

using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps
{
    public sealed class BuildContext : FrostingContext
    {
        // ---------------- Fields ----------------

        private const string dotnetVersion = "net10.0";

        // ---------------- Lifetime ----------------

        public BuildContext( ICakeContext context ) :
            base( context )
        {
            this.RepoRoot = context.Environment.WorkingDirectory;
            this.PretzelDir = this.RepoRoot.Combine( "_pretzel" );
            this.PretzelSln = this.PretzelDir.CombineWithFilePath( "src/Pretzel.sln" );
            this.PretzelExe = this.PretzelDir.CombineWithFilePath( $"src/Pretzel/bin/Debug/{dotnetVersion}/Pretzel.dll" );

            this.PostsDir = this.RepoRoot.Combine( "_posts" );

            this.PluginsDir = this.RepoRoot.Combine( "_plugins" );
            this.CategoryPluginOutput = this.PluginsDir.CombineWithFilePath( "Pretzel.Categories.dll" );
            this.SethExtensionsPluginOutput = this.PluginsDir.CombineWithFilePath( "Pretzel.SethExtensions.dll" );

            this.SiteOutput = this.RepoRoot.Combine( "_site" );
        }

        // ---------------- Properties ----------------

        public DirectoryPath RepoRoot { get; }

        public DirectoryPath PretzelDir { get; }

        public FilePath PretzelSln { get; }

        public FilePath PretzelExe { get; }

        public DirectoryPath PostsDir { get; }

        public DirectoryPath PluginsDir { get; }

        public FilePath CategoryPluginOutput { get; }

        public FilePath SethExtensionsPluginOutput { get; }

        public DirectoryPath SiteOutput { get; }

        // ---------------- Methods ----------------

        public void CheckPretzelDependency()
        {
            if(
                ( this.FileExists( this.PretzelExe ) == false ) ||
                ( this.FileExists( this.CategoryPluginOutput ) == false ) ||
                ( this.FileExists( this.SethExtensionsPluginOutput ) == false )
            )
            {
                BuildPretzel();
            }
        }

        public void BuildPretzel()
        {
            this.Information( "Building Pretzel..." );

            var msBuildSettings = new DotNetMSBuildSettings();
            msBuildSettings.Properties["DefineConstants"] = new List<string> { "NO_EXPORT_SETH_MARKDOWN_ENGINE" };

            var settings = new DotNetBuildSettings
            {
                Configuration = "Debug",
                MSBuildSettings = msBuildSettings
            };

            this.DotNetBuild( this.PretzelSln.ToString(), settings );

            this.EnsureDirectoryExists( this.PluginsDir );

            // Move Pretzel.Categories
            {
                FilePathCollection files = this.GetFiles(
                    this.PretzelDir.CombineWithFilePath(
                        $"src/Pretzel.Categories/bin/Debug/{dotnetVersion}/Pretzel.Categories.*"
                    ).ToString()
                );
                this.CopyFiles( files, this.PluginsDir );
            }

            // Move Pretzel.SethExtensions
            {
                FilePathCollection files = this.GetFiles(
                    this.PretzelDir.CombineWithFilePath(
                        $"src/Pretzel.SethExtensions/bin/Debug/{dotnetVersion}/Pretzel.SethExtensions.*"
                    ).ToString()
                );
                this.CopyFiles( files, this.PluginsDir );
            }

            {
                FilePathCollection files = this.GetFiles(
                    this.PretzelDir.CombineWithFilePath(
                        $"src/Pretzel.SethExtensions/bin/Debug/{dotnetVersion}/KristofferStrube.ActivityStreams.*"
                    ).ToString()
                );
                this.CopyFiles( files, this.PluginsDir );
            }

            this.Information( "Building Pretzel... Done!" );
        }

        public void RunPretzel( string argument, bool abortOnFail )
        {
            CheckPretzelDependency();
 
            bool fail = false;
            string onStdOut( string line )
            {
                if( string.IsNullOrWhiteSpace( line ) )
                {
                    return line;
                }
                else if( line.StartsWith( "Failed to render template" ) )
                {
                    fail = true;
                }

                Console.WriteLine( line );

                return line;
            }

            var settings = new ProcessSettings
            {
                Arguments = ProcessArgumentBuilder.FromString( $"\"{this.PretzelExe}\" {argument} --debug" ),
                Silent = false,
                RedirectStandardOutput = abortOnFail,
                RedirectedStandardOutputHandler = onStdOut
            };

            int exitCode = this.StartProcess( "dotnet", settings );
            if( exitCode != 0 )
            {
                throw new Exception( $"Pretzel exited with exit code: {exitCode}" );
            }

            if( abortOnFail && fail )
            {
                throw new Exception( "Failed to render template" );
            }
        }
    }
}
