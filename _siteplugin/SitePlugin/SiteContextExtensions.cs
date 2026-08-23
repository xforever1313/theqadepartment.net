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

using Pretzel.Logic.Templating.Context;

namespace SitePlugin
{
    public static class SiteContextExtensions
    {
        public static Page? TryGetFirstPost( this SiteContext siteContext )
        {
            return siteContext.Posts.OrderBy( p => p.Date ).FirstOrDefault();
        }

        public static Page? TryGetLastPost( this SiteContext siteContext )
        {
            return siteContext.Posts.OrderByDescending( p => p.Date ).FirstOrDefault();
        }
    }
}