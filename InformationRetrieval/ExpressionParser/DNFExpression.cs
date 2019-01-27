/*
(c) 2019 by Florian Scholz
This file is part of InformationRetrieval.

    InformationRetrieval is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    InformationRetrieval is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with InformationRetrieval.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;

namespace InformationRetrieval.ExpressionParser
{
    /// <summary>
    /// Provides a DTO representation of a boolean expression in DNF
    /// </summary>
    public class DNFExpression
    {
        /// <summary>
        /// Stores the AND expressions.
        /// </summary>
        public List<Conjunction> Conjunctions { get; set; } = new List<Conjunction>();
    }
}